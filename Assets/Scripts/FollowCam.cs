using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
  public float moveSpeed;
  private GameObject head;
  private Camera cam;
  // Start is called before the first frame update
  void Start()
  {
    head = GameObject.FindWithTag("Head");
    cam = this.GetComponent<Camera>();
    this.transform.position = new Vector3(head.transform.position.x, head.transform.position.y, -10);
  }

  // Update is called once per frame
  void Update()
  {
    var newX = Mathf.Lerp(this.transform.position.x, head.transform.position.x, Time.deltaTime * moveSpeed);
    var newY = Mathf.Lerp(this.transform.position.y, head.transform.position.y, Time.deltaTime * moveSpeed);
    this.transform.position = new Vector3(newX, newY, -10);
  }
}
