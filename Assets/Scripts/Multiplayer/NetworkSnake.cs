using Mirror;
using UnityEngine;
namespace Multiplayer {
    public class NetworkSnake : NetworkBehaviour {
        [SyncVar]
        private bool moved = false;
        [SyncVar]
        private Vector2 dir = Vector2.right;
        [SyncVar]
        private string nickname;

        [SyncVar]
        private bool ate = false;

        private Transform Head;
        public GameObject tailPrefab;
        public string Nickname {
            get => this.nickname;
            set => this.nickname = value;
        }
        private SyncList<Vector2> tails = new SyncList<Vector2>();
        public SyncList<Vector2> GetTails() {
            return tails;
        }
        // Start is called before the first frame update
        void Start() {
            Head = gameObject.transform.Find("Head");
            if (isServer) {
                for (int i = 1; i < gameObject.transform.childCount; i++) {
                    tails.Add(gameObject.transform.GetChild(i).position);
                }
            } else {
                CmdInitRequest();
            }
        }
        public override void OnStartLocalPlayer() {
            base.OnStartLocalPlayer();

            InvokeRepeating("Move", 0.3f, 0.3f);
        }
        void Move() {
            if (isServer) {
                Vector2 v = Head.position;
                Head.Translate(dir);
                if (ate) {
                    GameObject g = Instantiate(tailPrefab, v, Quaternion.identity, transform);
                    g.transform.SetAsFirstSibling();
                    var networkTransformChild = gameObject.AddComponent<NetworkTransformChild>();
                    networkTransformChild.clientAuthority = true;
                    networkTransformChild.target = g.transform;
                    tails.Insert(0, g.transform.position);
                    ate = false;
                } else if (tails.Count > 0) {
                    transform.GetChild(transform.childCount - 1).position = v;
                    transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
                    tails[tails.Count - 1] = v;
                    tails.Insert(0, tails[tails.Count - 1]);
                    tails.RemoveAt(tails.Count - 1);
                }
                moved = false;
            } else {
                Vector2 v = Head.position;
                Head.Translate(dir);
                if (ate) {
                    CmdEatStatusRequest(v);
                } else if (tails.Count > 0) {
                    transform.GetChild(transform.childCount - 1).position = v;
                    transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
                    CmdSubmitPositionRequest(v);
                }

            }
        }
        [Command]
        void CmdEatStatusRequest(Vector2 v) {
            GameObject g = Instantiate(tailPrefab, v, Quaternion.identity, transform);
            g.transform.SetAsFirstSibling();
            var networkTransformChild = gameObject.AddComponent<NetworkTransformChild>();
            networkTransformChild.clientAuthority = true;
            networkTransformChild.target = g.transform;
            tails.Insert(0, g.transform.position);
            ate = false;
        }
        [Command]
        void CmdInitRequest() {
            for (int i = 1; i < gameObject.transform.childCount; i++) {
                tails.Add(gameObject.transform.GetChild(i).position);
            }
        }
        [Command]
        void CmdSubmitPositionRequest(Vector2 v) {
            tails[tails.Count - 1] = v;
            tails.Insert(0, tails[tails.Count - 1]);
            moved = false;
        }
        [Command]
        void CmdSubmitDirRequest(Vector2 v) {
            dir = v;
            moved = true;
        }
        // Update is called once per frame
        void Update() {
            if (!isLocalPlayer) return;
            if (!moved) {
                if (isServer) {
                    if (Input.GetKey(KeyCode.LeftArrow) && (dir == Vector2.up || dir == -Vector2.up)) {
                        dir = -Vector2.right;
                        moved = true;
                    }
                    if (Input.GetKey(KeyCode.RightArrow) && (dir == Vector2.up || dir == -Vector2.up)) {
                        dir = Vector2.right;
                        moved = true;
                    }
                    if (Input.GetKey(KeyCode.UpArrow) && (dir == Vector2.right || dir == -Vector2.right)) {
                        dir = Vector2.up;
                        moved = true;
                    }
                    if (Input.GetKey(KeyCode.DownArrow) && (dir == Vector2.right || dir == -Vector2.right)) {
                        dir = -Vector2.up;
                        moved = true;
                    }
                } else {
                    if (Input.GetKey(KeyCode.LeftArrow) && (dir == Vector2.up || dir == -Vector2.up)) {
                        CmdSubmitDirRequest(-Vector2.right);
                    }
                    if (Input.GetKey(KeyCode.RightArrow) && (dir == Vector2.up || dir == -Vector2.up)) {
                        CmdSubmitDirRequest(Vector2.right);
                    }
                    if (Input.GetKey(KeyCode.UpArrow) && (dir == Vector2.right || dir == -Vector2.right)) {
                        CmdSubmitDirRequest(Vector2.up);
                    }
                    if (Input.GetKey(KeyCode.DownArrow) && (dir == Vector2.right || dir == -Vector2.right)) {
                        CmdSubmitDirRequest(-Vector2.up);
                    }
                }
            }
        }
    }
}

