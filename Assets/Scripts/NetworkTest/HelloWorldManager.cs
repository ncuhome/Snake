using MLAPI;
using UnityEngine;

namespace HelloWorld
{
  public class HelloWorldManager : MonoBehaviour
  {
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
      GUILayout.BeginArea(new Rect(10, 10, 300, 300));
      if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
      {
        StartButtons();
      }
      else
      {
        StatusLabels();

        SubmitNewPosition();
      }

      GUILayout.EndArea();
    }
    static void StartButtons()
    {
      if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
      if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
      if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
      var mode = NetworkManager.Singleton.IsHost ?
      "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

      GUILayout.Label("Transport: " +
      NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
      GUILayout.Label("Mode: " + mode);
    }

    static void SubmitNewPosition()
    {
      if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
      {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
        out var networkedClient))
        {
          var player = networkedClient.PlayerObject.GetComponent<HelloWorldPlayer>();
          if (player)
          {
            player.Move();
          }
        }
      }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}
