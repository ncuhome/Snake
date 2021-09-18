using System.Collections;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelGUI : MonoBehaviour
{
  float CountDown;
  bool isTiming;

  public static bool isInLevelSelector = false;

  public GameObject tipPanel;
  public GameObject gui;
  public GameObject homeCanvas;
  public GameObject content;
  public GameObject levelButtonPrefab;
  public GameObject panel;
  // Start is called before the first frame update
  void Start()
  {
    if (isInLevelSelector)
    {
      enterLevelSelector();
    }
  }
  void LoadLevels()
  {
    // DirectoryInfo root = new DirectoryInfo("./Assets/Resources/Levels");
    var files = Resources.LoadAll("Levels");
    GlobalManager.totalLevel = files.Length;
    // FileInfo[] files = root.GetFiles();

    float y = -100;
    var rectTransform = gui.transform.Find("Scroll View").GetComponent<RectTransform>();
    //关键语句，获取canvas长度
    var width = rectTransform.rect.size.x;
    var buttonWidth = levelButtonPrefab.GetComponent<RectTransform>().rect.size.x;
    Debug.Log(width);
    float split = (width - buttonWidth * 5) / 6.0f;
    float x = split + buttonWidth / 2;
    int num = 0;
    Array.Sort(files, (a, b) => Convert.ToInt32(a.name) - Convert.ToInt32(b.name));
    foreach (var file in files)
    {
      GameObject button = Instantiate(levelButtonPrefab, content.transform);
      button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
      button.transform.Find("text").GetComponent<TextMeshProUGUI>().text = file.name;
      button.GetComponent<Button>().onClick.AddListener(() =>
      {
        SpawnMap.level = file.name;
        SceneManager.LoadScene("MainScene");
      });
      x += buttonWidth + split;
      num++;
      if (num >= 5)
      {
        num = 0;
        x = split + buttonWidth / 2;
        y -= buttonWidth + 50;
      }
    }
  }

  public void backToHome()
  {
    isInLevelSelector = false;
    gui.gameObject.SetActive(false);
    homeCanvas.SetActive(true);
    // SceneManager.LoadScene("Home");

  }
  public void enterLevelSelector()
  {
    isInLevelSelector = true;
    gui.gameObject.SetActive(true);
    LoadLevels();
    homeCanvas.SetActive(false);
  }
  void Update()
  {
    exitDetection();
  }
  public void showRule()
  {
    panel.SetActive(true);
  }
  public void hideRule()
  {
    panel.SetActive(false);
  }
  private IEnumerator closeTip()
  {
    yield return new WaitForSecondsRealtime(2.0f);
    var canvasGroup = tipPanel.GetComponent<CanvasGroup>();
    while (canvasGroup.alpha > 0)
    {
      canvasGroup.alpha -= 0.1f;
      yield return new WaitForSecondsRealtime(0.05f);
    }
    tipPanel.gameObject.SetActive(false);
    canvasGroup.alpha = 1f;
  }
  private void showTip(string tip)
  {
    StartCoroutine(closeTip());
    tipPanel.SetActive(true);
    tipPanel.transform.Find("tip").GetComponent<TextMeshProUGUI>().text = tip;
  }
  void exitDetection()
  {
    if (Input.GetKeyUp(KeyCode.Escape))
    {
      if (CountDown == 0)
      {
        CountDown = Time.time;
        isTiming = true;
        showTip("再次后退将退出游戏!");
      }
      else
      {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
      }
    }
    if (isTiming)
    {
      if (Time.time - CountDown > 2.0)
      {
        CountDown = 0;
        isTiming = false;
      }
    }
  }
}

