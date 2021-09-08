using Newtonsoft.Json;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RegisterInfo {
    public string username { get; set; }
    public string password { get; set; }
    public string nickname { get; set; }
}
public class UserInfo {
    public string nickname { get; set; }
}
public class LoginData {
    public long id { get; set; }
    public string token { get; set; }
    public UserInfo user_info { get; set; }

    public string username { get; set; }
}
public class LoginResponse {
    public int code { get; set; }
    public LoginData data { get; set; }
    public string message { get; set; }
}
public class RegisterResponse {
    public int code { get; set; }
    public string message { get; set; }
}
public class LoginInfo {
    public string username { get; set; }
    public string password { get; set; }
    public LoginInfo() {
        username = "guest";
        password = "12345678";
    }
    public LoginInfo(string username, string password) {
        this.username = username;
        this.password = password;
    }
}
public class MenuManager : MonoBehaviour {
    private static MenuManager instance;
    public static MenuManager Instance { get { return instance; } }
    private readonly string rootUrl = "https://snake-api.nspyf.top";
    // private string rootUrl = "http://localhost:8000/api/test";
    // private LoginResponse loginResponse;

    // private RegisterResponse registerResponse;
    public GameObject loginPanel;
    public GameObject registerPanel;

    public Transform tipPanel;

    private bool isTiming = false;
    private float CountDown;
    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
    void Start() {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        tipPanel.gameObject.SetActive(false);
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
    }
    public void onClickLoginPanelCancel() {
        GameObject.Find("Menu").GetComponent<CanvasGroup>().alpha = 1;
        loginPanel.SetActive(false);
    }
    public void onClickRegisterPanelCancel() {
        GameObject.Find("Menu").GetComponent<CanvasGroup>().alpha = 1;
        registerPanel.SetActive(false);
    }
    public void onClickOpenLoginPanel() {
        string username = PlayerPrefs.GetString("username");
        string password = PlayerPrefs.GetString("password");
        if (username != "" && password != "") {
            LoginInfo info = new LoginInfo(username, password);
            string jsonString = JsonConvert.SerializeObject(info);
            LoginResponse response = new LoginResponse();
            StartCoroutine(Post(rootUrl + "/login", jsonString, response));
            return;
        }
        GameObject.Find("Menu").GetComponent<CanvasGroup>().alpha = 0.4f;
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
    public void onClickOpenRegisterPanel() {
        GameObject.Find("Menu").GetComponent<CanvasGroup>().alpha = 0.4f;
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
    public void onClickSubmitRegistrationInfo() {
        string username = registerPanel.transform.Find("UsernameInput")
        .GetComponent<TMP_InputField>().text;
        string nickname = registerPanel.transform.Find("NicknameInput")
        .GetComponent<TMP_InputField>().text;
        string password = registerPanel.transform.Find("PasswordInput")
        .GetComponent<TMP_InputField>().text;
        string retypePassword = registerPanel.transform.Find("RepeatPasswordInput")
        .GetComponent<TMP_InputField>().text;
        if (username == "" || nickname == "" || password == "" || retypePassword == "") {
            showTip("用户名，密码，昵称，重复密码不得为空！");
        }

        if (password != retypePassword) {
            registerPanel.SetActive(false);
            showTip("您输入的两次密码不一致！");
            return;
        }
        RegisterInfo info = new RegisterInfo();
        info.username = username;
        info.nickname = nickname;
        info.password = password;
        string jsonString = JsonConvert.SerializeObject(info, Formatting.Indented);
        Debug.Log(jsonString);
        RegisterResponse response = new RegisterResponse();
        StartCoroutine(Post(rootUrl + "/register", jsonString, response));
    }
    public void onClickSubmitLoginInfo() {
        string username = loginPanel.transform.Find("UsernameInput")
        .GetComponent<TMP_InputField>().text;
        string password = loginPanel.transform.Find("PasswordInput")
        .GetComponent<TMP_InputField>().text;
        if (username == "" || password == "") {
            showTip("用户名和密码不能为空！");
            return;
        }
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("password", password);
        PlayerPrefs.Save();
        LoginInfo info = new LoginInfo();
        info.username = username;
        info.password = password;
        string jsonString = JsonConvert.SerializeObject(info, Formatting.Indented);
        Debug.Log(jsonString);
        LoginResponse response = new LoginResponse();
        StartCoroutine(Post(rootUrl + "/login", jsonString, response));
        loginPanel.SetActive(false);
    }

    private void showTip(string tip) {
        StartCoroutine(closeTip());
        tipPanel.gameObject.SetActive(true);
        tipPanel.Find("tip").GetComponent<TextMeshProUGUI>().text = tip;
    }

    private void finishRequest<T>(T response) {
        switch (typeof(T).ToString()) {
            case "RegisterResponse": {
                    var res = response as RegisterResponse;
                    showTip(res.message);
                    registerPanel.SetActive(false);
                    loginPanel.SetActive(true);
                    break;
                }
            case "LoginResponse": {
                    var res = response as LoginResponse;
                    if (res.code != 0) {
                        showTip(res.message);
                        PlayerPrefs.DeleteKey("username");
                        PlayerPrefs.DeleteKey("password");
                        PlayerPrefs.Save();
                        return;
                    }
                    showTip("登陆成功！");
                    PlayerPrefs.SetString("token", res.data.token);
                    PlayerPrefs.SetString("nickname", res.data.user_info.nickname);
                    SceneManager.LoadSceneAsync("Menu");
                    break;
                }
        }
    }

    private IEnumerator closeTip() {
        yield return new WaitForSecondsRealtime(2.0f);
        var canvasGroup = tipPanel.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0) {
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        tipPanel.gameObject.SetActive(false);
        canvasGroup.alpha = 1f;
    }
    IEnumerator Post<T>(string url, string bodyJsonString, T response) {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Error while sending: " + request.error);
        }
        Debug.Log("Received: " + request.downloadHandler.text);
        response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        finishRequest(response);
    }

    void Update() {
        exitDetection();
    }
    void exitDetection() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (CountDown == 0) {
                CountDown = Time.time;
                isTiming = true;
                showTip("再次后退将退出游戏!");
            } else {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            }
        }
        if (isTiming) {
            if (Time.time - CountDown > 2.0) {
                CountDown = 0;
                isTiming = false;
            }
        }
    }
}
