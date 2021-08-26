using System;
using System.IO;
using UnityEngine;

//建立一个方法从文件中读取地图然后进行加载，同时控制相机距离来调整相应的地图缩放
public class SpawnMap : MonoBehaviour
{
  public GameObject wallPrefab;
  public GameObject foodPrefab;
  public GameObject headPrefab;

  public GameObject mainCamera;
  // public GameObject backgroundPrefab;

  public static string level;
  private static SpawnMap instance;
  public static SpawnMap Instance
  {
    get { return instance; }
  }

  // //borders
  // public Transform borderTop;
  // public Transform borderBottom;
  // public Transform borderLeft;
  // public Transform borderRight;

  // public float rewardProb = 0.01f;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }
  void Start(){
    LoadMap("Levels/" + level);
  }
  // Start is called before the first frame update

  public void LoadMap(string filename)
  {
    var file = Resources.Load<TextAsset>(filename);
    using (StringReader sr = new StringReader(file.text))
    {
      string line;
      line = sr.ReadLine();
      string[] param = line.Split(' ');
      int row = Convert.ToInt32(param[0]);
      int column = Convert.ToInt32(param[1]);
      float x = (column - 1) / 2.0f;
      float y = (row - 1) / 2.0f;
      mainCamera.transform.position = new Vector3(x, y, -10);
      mainCamera.GetComponent<Camera>().orthographicSize = y + 0.5f;
      for (int i = row - 1; i >= 0; i--)
      {
        line = sr.ReadLine();
        char[] sprites = line.ToCharArray();
        for (int j = 0; j < column; j++)
        {
          switch (sprites[j])
          {
            case '*':
              {
                Instantiate(wallPrefab, new Vector2(j, i), Quaternion.identity);
                break;
              }
            case 'f':
              {
                Instantiate(foodPrefab, new Vector2(j, i), Quaternion.identity);
                break;
              }
            case 's':
              {
                Instantiate(headPrefab, new Vector2(j, i), Quaternion.identity);
                break;
              }
            case '.':
              {
                //等待地板材质
                break;
              }
          }
        }
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
  // public void Spawn()
  // {
  //   int x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);

  //   int y = (int)Random.Range(borderBottom.position.y, borderTop.position.y);

  //   RaycastHit2D hit = Physics2D.BoxCast(new Vector2(x, y), new Vector2(1, 1), 0, new Vector2(1, 0));

  //   while (hit.collider.name.StartsWith("FoodPrefab") || hit.collider.name.StartsWith("RewardPrefab"))
  //   {
  //     x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);

  //     y = (int)Random.Range(borderBottom.position.y, borderTop.position.y);

  //     hit = Physics2D.BoxCast(new Vector2(x, y), new Vector2(1, 1), 0, new Vector2(1, 0));
  //   }


  //   Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
  // }
}
