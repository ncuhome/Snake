using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDetector : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  void OnTriggerStay2D(Collider2D other)
  {
    if (other.name.StartsWith("TailPrefab") || other.name.StartsWith("Head") || other.name.StartsWith("MonsterPrefab"))
    {
      SpawnFood.Instance.Spawn();
      Destroy(this);
    }
  }
}
