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
    DirectoryInfo root = new DirectoryInfo("./Assets/Levels");
    FileInfo[] files = root.GetFiles();

    int y = -100;
    var rectTransform = transform.Find("Scroll View").GetComponent<RectTransform>();
    //关键语句，获取canvas长度
    var width = rectTransform.rect.width * GetComponent<Canvas>().scaleFactor;
    float split = (width - 1500) / 6.0f;
    float x = 150 + split;
    int num = 0;
    foreach (FileInfo file in files)
    {
      if (Regex.IsMatch(file.Name, @"([1-9][0-9]*).txt$"))
      {
        var match = Regex.Match(file.Name, @"([1-9][0-9]*).txt$");
        GameObject button = Instantiate(levelButtonPrefab, content.transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        button.transform.Find("text").GetComponent<TextMeshProUGUI>().text = match.Groups[1].Value;
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
          SpawnMap.level = match.Groups[1].Value;
          SceneManager.LoadScene("MainScene");
        });
        x += 300 + split;
        num++;
        if (num >= 5)
        {
          num = 0;
          x = 150 + split;
          y += 50;
        }
      }
    }
  }
  public void backToHome(){
    SceneManager.LoadScene("Home");
  }
}
