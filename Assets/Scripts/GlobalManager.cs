using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
  public GameObject pauseMenu;
  public GameObject loseMenu;
  public GameObject gui;
  private static GlobalManager _instance;
  public static GlobalManager Instance
  {
    get { return _instance; }
  }
  private bool paused = false;

  private float currentTimeScale;

  public void updateScore(int score)
  {
    GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text = "Score:" + score.ToString();
  }

  public bool getPaused()
  {
    return paused;
  }
  void Awake()
  {
    if (_instance == null)
    {
      _instance = this;
    }
  }
  void Start()
  {
    pauseMenu.SetActive(false);
    loseMenu.SetActive(false);
  }

  public void dead()
  {
    gui.SetActive(false);
    loseMenu.SetActive(true);
    Time.timeScale = 0;
  }
  public void pause()
  {
    gui.SetActive(false);
    pauseMenu.SetActive(true);
    currentTimeScale = Time.timeScale;
    paused = true;
    Time.timeScale = 0;
  }
  public void resume()
  {
    pauseMenu.SetActive(false);
    gui.SetActive(true);
    paused = false;
    Time.timeScale = currentTimeScale;
  }
  public void restart()
  {
    SceneManager.LoadScene("MainScene");
    Time.timeScale = 1;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
