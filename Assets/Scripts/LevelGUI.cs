using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelGUI : MonoBehaviour
{
  int screenWidth;
  int screenHeight;

  public GameObject content;
  public GameObject levelButtonPrefab;
  // Start is called before the first frame update
  void Start()
  {
    screenWidth = Screen.width;
    screenHeight = Screen.height;
    LoadLevels();
  }
  void LoadLevels()
  {
    // DirectoryInfo root = new DirectoryInfo("./Assets/Resources/Levels");
    var files = Resources.LoadAll("Levels");
    // FileInfo[] files = root.GetFiles();

    float y = -100;
    var rectTransform = transform.Find("Scroll View").GetComponent<RectTransform>();
    //关键语句，获取canvas长度
    var width = rectTransform.rect.size.x;
    var buttonWidth = levelButtonPrefab.GetComponent<RectTransform>().rect.size.x;
    Debug.Log(width);
    float split = (width - buttonWidth * 5) / 6.0f;
    float x = split + buttonWidth / 2;
    int num = 0;
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
    SceneManager.LoadScene("Home");
  }
}

