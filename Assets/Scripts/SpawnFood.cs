using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    //food prefabs
    public GameObject foodPrefab;
    
    //borders
    public Transform borderTop;
    public Transform borderBottom;
    public Transform borderLeft;
    public Transform borderRight;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn",3,2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Spawn(){
      int x=(int)Random.Range(borderLeft.position.x, borderRight.position.x);

      int y=(int)Random.Range(borderBottom.position.y, borderTop.position.y);

      Instantiate(foodPrefab,new Vector2(x,y),Quaternion.identity);
    }
}
