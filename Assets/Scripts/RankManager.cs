using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
public class gameRecord
{
  public long id { get; set; }
  public long user_id { get; set; }
  public int score { get; set; }
}
public class User
{
  public long user_id { get; set; }
  public string nickname { get; set; }
}
public class RankData
{
  public gameRecord game_record { get; set; }
  public User user;
}
public class RankResponse
{
  public int code { get; set; }
  public string message { get; set; }
  public List<RankData> data { get; set; }
}
public class RankManager : MonoBehaviour
{

  public Transform rankUI;
  public GameObject item;
  private static RankManager instance;
  public static RankManager Instance
  {
    get
    {
      return instance;
    }
  }
  // Start is called before the first frame update
  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }

  void finishRequest(RankResponse response)
  {
    int rank = 0;
    float posY = 117;
    Transform content = rankUI.Find("Scroll View/Viewport/Content");
    foreach (var itemData in response.data)
    {
      GameObject obj = Instantiate(item, content);
      obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
      posY -= obj.GetComponent<RectTransform>().rect.height;
      var rankComponent = obj.transform.Find("rank").GetComponent<TextMeshProUGUI>();
      var nicknameComponent = obj.transform.Find("nickname").GetComponent<TextMeshProUGUI>();
      var scoreComponent = obj.transform.Find("score").GetComponent<TextMeshProUGUI>();
      rankComponent.text = (++rank).ToString();
      nicknameComponent.text = itemData.user.nickname;
      scoreComponent.text = itemData.game_record.score.ToString();
    }
  }
  public IEnumerator getRank(RankResponse response)
  {
    UnityWebRequest request = UnityWebRequest.Get("https://snake-api.nspyf.top/game/rank");
    yield return request.SendWebRequest();
    if (request.result == UnityWebRequest.Result.ConnectionError)
    {
      Debug.Log("Error while sending: " + request.error);
    }
    Debug.Log("Received: " + request.downloadHandler.text);
    response = JsonConvert.DeserializeObject<RankResponse>(request.downloadHandler.text);
    finishRequest(response);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
