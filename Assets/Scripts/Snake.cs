using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
  Vector2 dir = Vector2.right;
  List<Transform> tail = new List<Transform>();

  bool ate = false;

  int score=0;
  string rawText="Score:";

  public GameObject menu;
  public GameObject tailPrefab;
  // Start is called before the first frame update
  void Start()
  {
    menu.SetActive(false);
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
    if (Input.GetKey(KeyCode.RightArrow) && (dir == Vector2.up || dir == -Vector2.up))
      dir = Vector2.right;
    else if (Input.GetKey(KeyCode.DownArrow) && (dir == Vector2.right || dir == -Vector2.right))
      dir = -Vector2.up;
    else if (Input.GetKey(KeyCode.LeftArrow) && (dir == Vector2.up || dir == -Vector2.up))
      dir = -Vector2.right;
    else if (Input.GetKey(KeyCode.UpArrow) && (dir == Vector2.right || dir == -Vector2.right))
      dir = Vector2.up;
  }
  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.name.StartsWith("FoodPrefab"))
    {
      ate = true;

      Destroy(other.gameObject);
      score+=10;
      GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text=rawText+score.ToString();
    }
    else
    {
      Time.timeScale = 0;
      menu.SetActive(true);
    }
  }
  void Move()
  {
    Vector2 v = transform.position;

    transform.Translate(dir);

    if (ate)
    {
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
  }
}
