using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
  private static GlobalManager _instance;
  public static GlobalManager Instance
  {
    get { return _instance; }
  }
  void Awake()
  {
    if (_instance == null)
    {
      _instance = this;
    }
  }
  public void Restart()
  {
    SceneManager.LoadScene("MainScene");
    Time.timeScale = 1;
  }
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
