using System.Collections.Generic;
using System.Linq;
using static System.Math;
using UnityEngine;

public class Snake : MonoBehaviour
{
  Vector2 dir;
  List<Transform> tail = new List<Transform>();

  private bool ate = false, dead = false;

  private bool isFallBack = true;
  public bool IsFallBack { get => isFallBack; }

  private int score = 0;

  //运行时间，以1为起始,用于时间加速
  // private float runningTime = 1f;

  public AudioClip eatClip;

  public GameObject foodPrefab;
  //时间加速单位
  // public double boostScale = 1.1f;

  public GameObject tailPrefab;
  // Start is called before the first frame update

  //手机滑屏触发输入距离的平方
  private float minDistance = 1.0f;
  private Vector2 tailEndPos;

  private Vector2 lastDir;

  private bool lastAte;
  // private Coroutine monsterCanBeEatenCoroutine;

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
  }

  // Update is called once per frame
  void Update()
  {
    if (!dead && !GlobalManager.Instance.getPaused())
    {
      //一次输入后，在移动之前，使输入无效化，防止出现同时按下两个键，在移动判断之前导致dir变换两个方向，导致自己撞自己的bug产生而死亡

      #region
      //桌面端输入控制
      if (Input.GetKeyDown(KeyCode.RightArrow) && (dir != -Vector2.right || tail.Count == 0))
      {
        lastDir = dir;
        dir = Vector2.right;
        Move();
      }
      else if (Input.GetKeyDown(KeyCode.DownArrow) && (dir != Vector2.up || tail.Count == 0))
      {
        lastDir = dir;
        dir = -Vector2.up;
        Move();
      }
      else if (Input.GetKeyDown(KeyCode.LeftArrow) && (dir != Vector2.right || tail.Count == 0))
      {
        lastDir = dir;
        dir = -Vector2.right;
        Move();
      }
      else if (Input.GetKeyDown(KeyCode.UpArrow) && (dir != -Vector2.up || tail.Count == 0))
      {
        lastDir = dir;
        dir = Vector2.up;
        Move();
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
              if (deltaDir.x > 0 && (dir != -Vector2.right || tail.Count == 0))
              {
                lastDir = dir;
                dir = Vector2.right;
                Move();
              }
              if (deltaDir.x < 0 && (dir != Vector2.right || tail.Count == 0))
              {
                lastDir = dir;
                dir = -Vector2.right;
                Move();
              }
            }
            if (Mathf.Abs(deltaDir.y) > Mathf.Abs(deltaDir.x))
            {
              if (deltaDir.y > 0 && (dir != -Vector2.up || tail.Count == 0))
              {
                lastDir = dir;
                dir = Vector2.up;
                Move();
              }
              if (deltaDir.y < 0 && (dir != Vector2.up || tail.Count == 0))
              {
                lastDir = dir;
                dir = -Vector2.up;
                Move();
              }
            }
          }
        }
        #endregion
      }
      // runningTime += Time.deltaTime;
      // Time.timeScale = (float)Pow(boostScale, Log(runningTime));
    }
  }
  #region 
  //虚拟按键控制,放在这方便控制方向变量
  //但是挂载函数放在了globalManager
  public void onClickLeft()
  {
    if (dir != Vector2.right || tail.Count == 0)
    {
      lastDir = dir;
      dir = -Vector2.right;
      Move();
    }
  }
  public void onClickRight()
  {
    if (dir != -Vector2.right || tail.Count == 0)
    {
      lastDir = dir;
      dir = Vector2.right;
      Move();
    }
  }
  public void onClickUp()
  {
    if (dir != -Vector2.up || tail.Count == 0)
    {
      lastDir = dir;
      dir = Vector2.up;
      Move();
    }
  }
  public void onClickDown()
  {
    if (dir != Vector2.up || tail.Count == 0)
    {
      lastDir = dir;
      dir = -Vector2.up;
      Move();
    }
  }
  #endregion
  void OnTriggerEnter2D(Collider2D other)
  {
    // if (other.name.StartsWith("FoodPrefab") || other.name.StartsWith("RewardPrefab") || (other.name.StartsWith("MonsterPrefab") && other.GetComponent<EnemyMove>().canBeEaten))
    if (other.name.StartsWith("FoodPrefab") || other.name.StartsWith("RewardPrefab"))
    {
      ate = true;
      AudioSource.PlayClipAtPoint(eatClip, new Vector3(0, 0, -10));
      if (other.name.StartsWith("RewardPrefab"))
      {
        // if (monsterCanBeEatenCoroutine != null) StopCoroutine(monsterCanBeEatenCoroutine);
        // monsterCanBeEatenCoroutine = StartCoroutine(GlobalManager.Instance.monsterEnterCanBeEatenStatus());
        score += 50;
        // Time.timeScale -= 1.2f;
      }
      // else if (other.name.StartsWith("MonsterPrefab"))
      // {
      //   var coroutines = GlobalManager.Instance.getCoroutines();
      //   if (coroutines.ContainsKey(other.gameObject))
      //   {
      //     GlobalManager.Instance.StopCoroutine(coroutines[other.gameObject]);
      //     coroutines.Remove(other.gameObject);
      //   }
      //   score += 100;
      // }
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

    lastAte = ate;
    if (ate)
    {
      GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

      tail.Insert(0, g.transform);

      ate = false;
    }
    else if (tail.Count > 0)
    {
      tailEndPos = tail.Last().position;
      tail.Last().position = v;

      tail.Insert(0, tail.Last());
      tail.RemoveAt(tail.Count - 1);
    }
    isFallBack = false;
    // moved = false;
  }

  public void FallBack()
  {
    //todo:考虑吃掉食物的那一次移动，此时长度还未增长并且ate==true，还需要增加一个缓存吃掉的食物的操作，
    //然后正常移动相对来讲比较好回溯，吃掉食物之后再移动了一格的话，就得将头部前面的第一个尾巴删除,然后还得将ate状态置为true
    if (lastAte)
    {
      //连续吃两个食物
      if (ate)
      {
        lastAte = false;
        Vector2 headPos = transform.position;
        var tailObj = tail.First().gameObject;
        tail.RemoveAt(0);
        Destroy(tailObj);
        transform.Translate(-dir);
        Instantiate(foodPrefab, headPos, Quaternion.identity);
      }
      //只吃了一个食物
      else
      {
        lastAte = false;
        ate = true;
        var tailObj = tail.First().gameObject;
        tail.RemoveAt(0);
        Destroy(tailObj);
        transform.Translate(-dir);
      }
    }
    else if (tail.Count > 0)
    {
      if (ate)
      {
        ate = false;
        var tailFirst = tail.First();
        Vector2 headPos = transform.position;
        tailFirst.position = tailEndPos;
        tail.Add(tailFirst);
        tail.RemoveAt(0);
        transform.Translate(-dir);
        Instantiate(foodPrefab, headPos, Quaternion.identity);
      }
      else
      {
        var tailFirst = tail.First();
        tailFirst.position = tailEndPos;
        tail.Add(tailFirst);
        tail.RemoveAt(0);
        transform.Translate(-dir);
      }
    }
    else
    {
      if (ate)
      {
        ate = false;
        Vector2 headPos = transform.position;
        transform.Translate(-dir);
        Instantiate(foodPrefab, headPos, Quaternion.identity);
      }
      else
      {
        transform.Translate(-dir);
      }
    }
    dir = lastDir;
    isFallBack = true;
  }

  private bool isDead(Vector2 dir)
  {
    Vector2 pos = transform.position;
    //从pos+dir向pos发射一条射线
    RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
    // if (hit.collider.name.StartsWith("TailPrefab") || (hit.collider.name.StartsWith("MonsterPrefab") && !hit.collider.GetComponent<EnemyMove>().canBeEaten)) return true;
    if (hit.collider.name.StartsWith("TailPrefab")) return true;
    if (hit.collider.transform.parent == null) return false;
    return hit.collider.transform.parent.name == "Wall";
  }
}
