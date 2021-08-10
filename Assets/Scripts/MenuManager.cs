using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class RegisterInfo
{
  public string username;
  public string password;
  public string nickname;
}
public class MenuManager : MonoBehaviour
{
  private string rootUrl = "https://snake-api.nspyf.top";
  // private string rootUrl = "http://localhost:8000/api/test";
  public GameObject loginPanel;
  public GameObject registerPanel;
  void Start()
  {
    loginPanel.SetActive(false);
    registerPanel.SetActive(false);
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
    .Find("Text Area")
    .Find("Username")
    .GetComponent<TextMeshProUGUI>().text;
    string nickname = registerPanel.transform.Find("NicknameInput")
    .Find("Text Area")
    .Find("Nickname")
  .GetComponent<TextMeshProUGUI>().text;
    string password = registerPanel.transform.Find("PasswordInput")
    .Find("Text Area")
    .Find("Password")
    .GetComponent<TextMeshProUGUI>().text;
    RegisterInfo info = new RegisterInfo();
    info.username = username;
    info.nickname = nickname;
    info.password = password;
    string jsonString = JsonUtility.ToJson(info);
    jsonString = jsonString.Replace("\u200B","");
    Debug.Log(jsonString);
    StartCoroutine(Post(rootUrl + "/register", jsonString));
  }

  IEnumerator Post(string url, string bodyJsonString)
  {
    var request = new UnityWebRequest(url, "POST");
    byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    yield return request.SendWebRequest();
    Debug.Log(request.responseCode);
    if (request.result == UnityWebRequest.Result.ConnectionError)
    {
      Debug.Log("Error while sending: " + request.error);
    }
    Debug.Log("Received: " + request.downloadHandler.text);
  }
}
