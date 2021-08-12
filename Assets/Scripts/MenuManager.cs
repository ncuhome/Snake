using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;

public class RegisterInfo
{
  public string username { get; set; }
  public string password { get; set; }
  public string nickname { get; set; }
}
public class UserInfo
{
  public string nickname { get; set; }
}
public class LoginData
{
  public long id { get; set; }
  public string token { get; set; }
  public UserInfo user_info { get; set; }

  public string username { get; set; }
}
public class LoginResponse
{
  public int code { get; set; }
  public LoginData data { get; set; }
  public string message { get; set; }
}
public class RegisterResponse
{
  public int code { get; set; }
  public string message { get; set; }
}
public class LoginInfo
{
  public string username { get; set; }
  public string password { get; set; }
}
public class MenuManager : MonoBehaviour
{
  private static MenuManager instance;
  public static MenuManager Instance { get { return instance; } }
  private string rootUrl = "https://snake-api.nspyf.top";
  // private string rootUrl = "http://localhost:8000/api/test";
  private LoginResponse loginResponse;

  private RegisterResponse registerResponse;
  public GameObject loginPanel;
  public GameObject registerPanel;

  public Transform tipPanel;
  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }
  void Start()
  {
    loginPanel.SetActive(false);
    registerPanel.SetActive(false);
    tipPanel.gameObject.SetActive(false);
    Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
  }
  public void onClickLoginPanelCancel()
  {
    loginPanel.SetActive(false);
  }
  public void onClickRegisterPanelCancel()
  {
    registerPanel.SetActive(false);
  }
  public void onClickOpenLoginPanel()
  {
    loginPanel.SetActive(true);
  }
  public void onClickOpenRegisterPanel()
  {
    registerPanel.SetActive(true);
  }
  public void onClickSubmitRegistrationInfo()
  {
    string username = registerPanel.transform.Find("UsernameInput")
    .GetComponent<TMP_InputField>().text;
    string nickname = registerPanel.transform.Find("NicknameInput")
    .GetComponent<TMP_InputField>().text;
    string password = registerPanel.transform.Find("PasswordInput")
    .GetComponent<TMP_InputField>().text;
    string retypePassword = registerPanel.transform.Find("RepeatPasswordInput")
    .GetComponent<TMP_InputField>().text;

    if (password != retypePassword)
    {
      registerPanel.SetActive(false);
      showTip("The two passwords you entered were inconsistent.");
      return;
    }
    RegisterInfo info = new RegisterInfo();
    info.username = username;
    info.nickname = nickname;
    info.password = password;
    string jsonString = JsonConvert.SerializeObject(info, Formatting.Indented);
    Debug.Log(jsonString);
    StartCoroutine(Post(rootUrl + "/register", jsonString, registerResponse));
  }
  public void onClickSubmitLoginInfo()
  {
    string username = loginPanel.transform.Find("UsernameInput")
    .GetComponent<TMP_InputField>().text;
    string password = loginPanel.transform.Find("PasswordInput")
    .GetComponent<TMP_InputField>().text;
    LoginInfo info = new LoginInfo();
    info.username = username;
    info.password = password;
    string jsonString = JsonConvert.SerializeObject(info, Formatting.Indented);
    //删除不可见字符
    Debug.Log(jsonString);
    StartCoroutine(Post(rootUrl + "/login", jsonString, loginResponse));
  }

  private void showTip(string tip)
  {
    tipPanel.gameObject.SetActive(true);
    tipPanel.Find("tip").GetComponent<TextMeshProUGUI>().text = tip;
    StartCoroutine(closeTip());
  }

  private void finishRequest<T>(T response)
  {
    ClassHelper.ForeachClassProperties(response);
    switch (typeof(T).ToString())
    {
      case "RegisterResponse":
        {
          var res = response as RegisterResponse;
          Debug.Log(res.message);
          break;
        }
      case "LoginResponse":
        {
          var res = response as LoginResponse;
          Debug.Log(res.data.token);
          Debug.Log(res.data.user_info.nickname);
          break;
        }
    }
  }

  private IEnumerator closeTip()
  {
    yield return new WaitForSecondsRealtime(3.0f);
    tipPanel.gameObject.SetActive(false);
  }
  IEnumerator Post<T>(string url, string bodyJsonString, T response)
  {
    var request = new UnityWebRequest(url, "POST");
    byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    yield return request.SendWebRequest();
    if (request.result == UnityWebRequest.Result.ConnectionError)
    {
      Debug.Log("Error while sending: " + request.error);
    }
    Debug.Log("Received: " + request.downloadHandler.text);
    response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
    finishRequest(response);
  }
}
