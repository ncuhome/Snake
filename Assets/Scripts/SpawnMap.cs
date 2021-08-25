using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo:建立一个方法从文件中读取地图然后进行加载，同时控制相机距离来调整相应的地图缩放
public class SpawnMap : MonoBehaviour
{
  private static SpawnMap instance;
  public static SpawnMap Instance
  {
    get { return instance; }
  }
  //food prefabs
  public GameObject foodPrefab;

  //borders
  public Transform borderTop;
  public Transform borderBottom;
  public Transform borderLeft;
  public Transform borderRight;

  public float rewardProb = 0.01f;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }
  // Start is called before the first frame update
  void Start()
  {
    InvokeRepeating("Spawn", 3, 4);
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void Spawn()
  {
    int x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);

    int y = (int)Random.Range(borderBottom.position.y, borderTop.position.y);
    
    RaycastHit2D hit = Physics2D.BoxCast(new Vector2(x, y), new Vector2(1, 1), 0, new Vector2(1, 0));

    while (hit.collider.name.StartsWith("FoodPrefab") || hit.collider.name.StartsWith("RewardPrefab"))
    {
      x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);

      y = (int)Random.Range(borderBottom.position.y, borderTop.position.y);
      
      hit = Physics2D.BoxCast(new Vector2(x, y), new Vector2(1, 1), 0, new Vector2(1, 0));
    }


    Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
  }
}
