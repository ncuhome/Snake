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
  public GameObject loseMenu;
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

  void finishRequest(RankResponse response, int score)
  {
    int rank = 0;
    float posY = 117;
    Transform content = rankUI.Find("Scroll View/Viewport/Content");
    int thisRank = getThisRank(response.data.ToArray(), score, 0, response.data.ToArray().Length);
    if (thisRank == -1)
      loseMenu.transform.Find("Rank/ScoreDisplay").GetComponent<TextMeshProUGUI>().text = "你的分数: " + score.ToString() + "  排名：" + thisRank.ToString() + "+";
    else
      loseMenu.transform.Find("Rank/ScoreDisplay").GetComponent<TextMeshProUGUI>().text = "你的分数: " + score.ToString() + "  排名：" + thisRank.ToString();
    loseMenu.SetActive(true);
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
  int getThisRank(RankData[] data, int score, int min, int max)
  {
    int mid = (min + max) / 2;
    if (data[mid].game_record.score == score) return mid + 1;
    if (mid + 1 >= data.Length) return -1;
    if (data[mid].game_record.score > score && data[mid + 1].game_record.score < score) return mid + 2;
    if (data[mid].game_record.score < score) return getThisRank(data, score, min, mid);
    if (data[mid].game_record.score > score) return getThisRank(data, score, mid, max);
    return -1;
  }
  public IEnumerator getRank(RankResponse response, int score)
  {
    yield return new WaitForSecondsRealtime(0.05f);
    UnityWebRequest request = UnityWebRequest.Get("https://snake-api.nspyf.top/game/rank");
    yield return request.SendWebRequest();
    if (request.result == UnityWebRequest.Result.ConnectionError)
    {
      Debug.Log("Error while sending: " + request.error);
    }
    Debug.Log("Received: " + request.downloadHandler.text);
    response = JsonConvert.DeserializeObject<RankResponse>(request.downloadHandler.text);
    finishRequest(response, score);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
