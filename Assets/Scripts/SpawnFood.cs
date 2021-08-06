using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
  private static SpawnFood instance;
  public static SpawnFood Instance
  {
    get { return instance; }
  }
  //food prefabs
  public GameObject foodPrefab;

  public GameObject rewardPrefab;

  //borders
  public Transform borderTop;
  public Transform borderBottom;
  public Transform borderLeft;
  public Transform borderRight;

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
    if (Random.Range(0.0f, 1.0f) < 0.2)
    {
      Instantiate(rewardPrefab,new Vector2(x,y),Quaternion.identity);
      return;
    }


    Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
  }
}
