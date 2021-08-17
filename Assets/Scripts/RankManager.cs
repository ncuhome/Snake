using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
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

  public Transform rank;
  public GameObject item;
  // Start is called before the first frame update
  void Start()
  {
    RankResponse response = new RankResponse();
    StartCoroutine(getRank(response));
  }

  void finishRequest(RankResponse response)
  {
    int rank = 0;
    foreach (var item in response.data)
    {
      Debug.Log(++rank);
      Debug.Log(item.user.nickname);
      Debug.Log(item.game_record.score);
    }
  }
  IEnumerator getRank(RankResponse response)
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
