using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
using UnityEngine;

public class Snake : MonoBehaviour
{
  Vector2 dir = Vector2.right;
  List<Transform> tail = new List<Transform>();

  private bool ate = false, dead = false, moved = false;

  private int score = 0;

  //运行时间，以1为起始,用于时间加速
  private float runningTime = 1f;

  public AudioClip eatClip;

  //时间加速单位
  public double boostScale = 1.1f;

  public Transform joyStick;

  public GameObject tailPrefab;
  // Start is called before the first frame update

  //手机滑屏触发输入距离的平方
  private float minDistance = 1.0f;
  private Coroutine monsterCanBeEatenCoroutine;

  private static Snake instance;
  public static Snake Instance
  {
    get { return instance; }
  }
  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }
  void Start()
  {
    Input.simulateMouseWithTouches = true;
    Input.multiTouchEnabled = true;
    for (int i = 1; i < 5; i++)
    {
      GameObject g = (GameObject)Instantiate(tailPrefab, new Vector2(this.transform.position.x - i, this.transform.position.y), Quaternion.identity);

      tail.Add(g.transform);
    }
    InvokeRepeating("Move", 0.3f, 0.3f);
  }

  // Update is called once per frame
  void Update()
  {
    if (!dead && !GlobalManager.Instance.getPaused())
    {
      //一次输入后，在移动之前，使输入无效化，防止出现同时按下两个键，在移动判断之前导致dir变换两个方向，导致自己撞自己的bug产生而死亡
      if (!moved)
      {
        #region
        //桌面端输入控制
        if (Input.GetKey(KeyCode.RightArrow) && (dir == Vector2.up || dir == -Vector2.up))
        {
          dir = Vector2.right;
          moved = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && (dir == Vector2.right || dir == -Vector2.right))
        {
          dir = -Vector2.up;
          moved = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && (dir == Vector2.up || dir == -Vector2.up))
        {
          dir = -Vector2.right;
          moved = true;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && (dir == Vector2.right || dir == -Vector2.right))
        {
          dir = Vector2.up;
          moved = true;
        }
        #endregion
        #region
        //手机端输入控制
        //滑屏控制
        if (!GlobalManager.Instance.getIsJoyStick())
        {
          if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
          {
            if (Vector2.SqrMagnitude(Input.GetTouch(0).deltaPosition) > minDistance)
            {
              Vector2 deltaDir = Input.GetTouch(0).deltaPosition;
              if (Mathf.Abs(deltaDir.x) > Mathf.Abs(deltaDir.y))
              {
                if (deltaDir.x > 0 && (dir == Vector2.up || dir == -Vector2.up))
                {
                  dir = Vector2.right;
                  moved = true;
                }
                if (deltaDir.x < 0 && (dir == Vector2.up || dir == -Vector2.up))
                {
                  dir = -Vector2.right;
                  moved = true;
                }
              }
              if (Mathf.Abs(deltaDir.y) > Mathf.Abs(deltaDir.x))
              {
                if (deltaDir.y > 0 && (dir == Vector2.right || dir == -Vector2.right))
                {
                  dir = Vector2.up;
                  moved = true;
                }
                if (deltaDir.y < 0 && (dir == Vector2.right || dir == -Vector2.right))
                {
                  dir = -Vector2.up;
                  moved = true;
                }
              }
            }
          }
        }
        #endregion
      }
      runningTime += Time.deltaTime;
      Time.timeScale = (float)Pow(boostScale, Log(runningTime));
    }
  }
  #region 
  //虚拟按键控制,放在这方便控制方向变量
  //但是挂载函数放在了globalManager
  public void onClickLeft()
  {
    if (!moved && (dir == Vector2.up || dir == -Vector2.up))
    {
      dir = -Vector2.right;
      moved = true;
    }
  }
  public void onClickRight()
  {
    if (!moved && (dir == Vector2.up || dir == -Vector2.up))
    {
      dir = Vector2.right;
      moved = true;
    }
  }
  public void onClickUp()
  {
    if (!moved && (dir == Vector2.right || dir == -Vector2.right))
    {
      dir = Vector2.up;
      moved = true;
    }
  }
  public void onClickDown()
  {
    if (!moved && (dir == Vector2.right || dir == -Vector2.right))
    {
      dir = -Vector2.up;
      moved = true;
    }
  }
  #endregion
  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.name.StartsWith("FoodPrefab") || other.name.StartsWith("RewardPrefab") || (other.name.StartsWith("MonsterPrefab") && other.GetComponent<EnemyMove>().canBeEaten))
    {
      ate = true;

      if (other.name.StartsWith("RewardPrefab"))
      {
        if (monsterCanBeEatenCoroutine != null) StopCoroutine(monsterCanBeEatenCoroutine);
        monsterCanBeEatenCoroutine = StartCoroutine(GlobalManager.Instance.monsterEnterCanBeEatenStatus());
        score += 50;
        Time.timeScale -= 1.2f;
      }
      else if (other.name.StartsWith("MonsterPrefab"))
      {
        var coroutines = GlobalManager.Instance.getCoroutines();
        if (coroutines.ContainsKey(other.gameObject))
        {
          GlobalManager.Instance.StopCoroutine(coroutines[other.gameObject]);
          coroutines.Remove(other.gameObject);
        }
        score += 100;
      }
      else
      {
        score += 10;
      }
      Destroy(other.gameObject);
      GlobalManager.Instance.updateScore(score);
    }
  }
  void Move()
  {
    if (dead)
    {
      return;
    }
    if (isDead(dir) && !dead)
    {
      dead = true;
      GlobalManager.Instance.dead(score);
      return;
    }
    Vector2 v = transform.position;

    transform.Translate(dir);

    if (ate)
    {
      AudioSource.PlayClipAtPoint(eatClip, new Vector3(0, 0, -10));
      GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

      tail.Insert(0, g.transform);

      ate = false;
    }
    else if (tail.Count > 0)
    {
      tail.Last().position = v;

      tail.Insert(0, tail.Last());
      tail.RemoveAt(tail.Count - 1);
    }
    moved = false;
  }

  private bool isDead(Vector2 dir)
  {
    Vector2 pos = transform.position;
    //从pos+dir向pos发射一条射线
    RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
    if (hit.collider.name.StartsWith("TailPrefab") || (hit.collider.name.StartsWith("MonsterPrefab") && !hit.collider.GetComponent<EnemyMove>().canBeEaten)) return true;
    if (hit.collider.transform.parent == null) return false;
    return hit.collider.transform.parent.name == "Wall";
  }
}
