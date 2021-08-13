using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GlobalManager : MonoBehaviour
{
  public GameObject pauseMenu;
  public GameObject loseMenu;
  public GameObject gui;

  private Dictionary<GameObject, Coroutine> coroutines=new Dictionary<GameObject, Coroutine>();

  public float fadeSpeed = 0.1f;
  public GameObject monsterPrefab;
  private static GlobalManager _instance;
  public static GlobalManager Instance
  {
    get { return _instance; }
  }
  private bool paused = false;

  private float currentTimeScale;

  public Dictionary<GameObject, Coroutine> getCoroutines() { return coroutines; }

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
    InvokeRepeating("instantiateMonster", 1.0f, 90.0f);
  }

  public IEnumerator monsterEnterCanBeEatenStatus()
  {
    StopAllCoroutines();
    coroutines.Clear();
    GameObject[] monsters = GameObject.FindGameObjectsWithTag("monster");
    foreach (GameObject monster in monsters)
    {
      monster.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
      monster.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.4f);
      monster.GetComponent<EnemyMove>().canBeEaten = true;
    }
    yield return new WaitForSecondsRealtime(4.0f);
    //考虑这四秒被吃掉的，那么就从那里面挑选剩下的
    monsters = GameObject.FindGameObjectsWithTag("monster");
    ArrayList monsterArray = new ArrayList();
    foreach (GameObject monster in monsters)
    {
      if (monster.GetComponent<EnemyMove>().canBeEaten)
      {
        monsterArray.Add(monster);
      }
    }
    monsters = (GameObject[])monsterArray.ToArray(typeof(GameObject));
    foreach (GameObject monster in monsters)
    {
      var coroutine = StartCoroutine(enterResumeStatus(monster));
      coroutines.Add(monster, coroutine);
    }
  }

  IEnumerator enterResumeStatus(GameObject monster)
  {
    var sprite = monster.GetComponent<SpriteRenderer>();
    int times = 5;
    while (--times > 0)
    {
      while (sprite.color.a < 1)
      {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + fadeSpeed);
        yield return new WaitForSecondsRealtime(0.05f);
        if (monster == null) yield break;
      }
      while (sprite.color.a > 0.4f)
      {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - fadeSpeed);
        yield return new WaitForSecondsRealtime(0.05f);
        if (monster == null) yield break;
      }
    }
    if (monster == null) yield break;
    while (sprite.color.a < 1)
    {
      sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + fadeSpeed);
      yield return new WaitForSecondsRealtime(0.05f);
      if (monster == null) yield break;
    }
    if (monster == null) yield break;
    monster.GetComponent<EnemyMove>().canBeEaten = false;
    monster.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
  }
  public void stopResume(Coroutine coroutine){
    StopCoroutine(coroutine);
  }

  void instantiateMonster()
  {
    Instantiate(monsterPrefab, new Vector2(-38, 20), Quaternion.identity);
  }

  public void dead(int score)
  {
    gui.SetActive(false);
    loseMenu.transform.Find("ScoreDisplay").GetComponent<TextMeshProUGUI>().text = "你的分数: " + score.ToString();
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
