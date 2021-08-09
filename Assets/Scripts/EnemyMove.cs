using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
  public GameObject[] wayPointsGo;
  private List<Vector3> wayPoints = new List<Vector3>();
  public float speed = 0.15f;

  private int index = 0;

  private Vector3 startPos;

  void Start()
  {
    startPos = transform.position + new Vector3(8, 0, 0);
    loadAPath(wayPointsGo[Random.Range(0, wayPointsGo.Length)]);

  }
  // Update is called once per frame
  void FixedUpdate()
  {
    if (transform.position != wayPoints[index])
    {
      Vector2 pos = Vector2.MoveTowards(transform.position, wayPoints[index], speed);
      GetComponent<Rigidbody2D>().MovePosition(pos);
    }
    else
    {
      index++;
      if (index >= wayPoints.Count)
      {
        index = 0;
        loadAPath(wayPointsGo[Random.Range(0, wayPointsGo.Length)]);
      }
    }

  }
  private void loadAPath(GameObject obj)
  {
    wayPoints.Clear();
    wayPoints.Add(startPos);
    foreach (Transform t in obj.transform)
    {
      wayPoints.Add(t.position);
    }
    wayPoints.Add(startPos);
  }
}
