using MLAPI;
using MLAPI.NetworkVariable.Collections;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace MultiPlayer
{
  public class NetworkSnake : NetworkBehaviour
  {
    private NetworkVariableVector2 dir = new NetworkVariableVector2(new NetworkVariableSettings
    {
      WritePermission = NetworkVariablePermission.ServerOnly,
      ReadPermission = NetworkVariablePermission.Everyone
    });
    private NetworkList<Transform> tail = new NetworkList<Transform>(new NetworkVariableSettings
    {
      WritePermission = NetworkVariablePermission.ServerOnly,
      ReadPermission = NetworkVariablePermission.Everyone
    });

    private NetworkVariableBool ate = new NetworkVariableBool(new NetworkVariableSettings
    {
      WritePermission = NetworkVariablePermission.ServerOnly,
      ReadPermission = NetworkVariablePermission.Everyone
    });
    public GameObject tailPrefab;

    //要建立输入如果是host则直接更改dir，不然则根据输入调用服务器rpc函数更改dir变量
    void Move()
    {
      //如果是host，就可以直接计算然后更新位置
      if (NetworkManager.Singleton.IsServer)
      {
        Vector2 position = this.transform.Find("Head").position;
        this.transform.Find("Head").Translate(dir.Value);
        if (ate.Value)
        {
          GameObject obj = Instantiate(tailPrefab, position, Quaternion.identity, this.transform);
          obj.GetComponent<NetworkObject>().Spawn(null, true);
          tail.Insert(0, obj.transform);
          ate.Value = false;
        }
        else if (tail.Count > 0)
        {
          tail[tail.Count - 1].position = position;
          tail.Insert(0, tail[tail.Count - 1]);
          tail.RemoveAt(tail.Count - 1);
        }
      }
      else
      {
        //todo:调用rpc更新位置插入等等操作
      }
    }
    // Start is called before the first frame update
    void Start()
    {
      InvokeRepeating("Move", 0.3f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}
