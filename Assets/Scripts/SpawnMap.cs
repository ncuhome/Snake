using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

//建立一个方法从文件中读取地图然后进行加载，同时控制相机距离来调整相应的地图缩放
public class SpawnMap : MonoBehaviour {
    public GameObject[] wallPrefabs;
    public GameObject[] foodPrefabs;
    public GameObject[] floorPrefabs;
    public GameObject headPrefab;

    public GameObject[] obstacles;
    public GameObject mainCamera;
    // public GameObject backgroundPrefab;

    public static string level;

    public int foodNum;
    private static SpawnMap instance;
    public static SpawnMap Instance {
        get { return instance; }
    }

    // //borders
    // public Transform borderTop;
    // public Transform borderBottom;
    // public Transform borderLeft;
    // public Transform borderRight;

    // public float rewardProb = 0.01f;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
    void Start() {
        LoadMap("Levels/" + level);
    }

    public void NextLevel() {
        var newLevel = Convert.ToInt32(level) + 1;
        level = newLevel.ToString();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    // Start is called before the first frame update

    public void LoadMap(string filename) {
        FallBack.FallBackManager.Instance.resetState();
        var file = Resources.Load<TextAsset>(filename);
        using (StringReader sr = new StringReader(file.text)) {
            string line;
            line = sr.ReadLine();
            string[] param = line.Split(' ');
            int row = Convert.ToInt32(param[0]);
            int column = Convert.ToInt32(param[1]);
            float x = (column - 1) / 2.0f;
            float y = (row - 1) / 2.0f;
            mainCamera.transform.position = new Vector3(x, y - 0.15f, -10);
            mainCamera.GetComponent<Camera>().orthographicSize = y + 0.65f;
            for (int i = row - 1; i >= 0; i--) {
                line = sr.ReadLine();
                char[] sprites = line.ToCharArray();
                for (int j = 0; j < column; j++) {
                    switch (sprites[j]) {
                        case '*': {
                                //top
                                if (i == row - 1) {
                                    if (j == 0)
                                        Instantiate(wallPrefabs[3], new Vector2(j, i), Quaternion.identity);
                                    else if (j == column - 1) {
                                        Instantiate(wallPrefabs[4], new Vector2(j, i), Quaternion.identity);
                                    } else {
                                        Instantiate(wallPrefabs[5], new Vector2(j, i), Quaternion.identity);
                                    }
                                    break;
                                }
                                //left
                                if (j == 0) {
                                    //shit texture
                                    if (i == 0) Instantiate(wallPrefabs[2], new Vector2(j, i - 0.15f), Quaternion.identity);
                                    else {
                                        Instantiate(wallPrefabs[1], new Vector2(j, i), Quaternion.identity);
                                    }
                                    break;
                                }
                                //bottom
                                if (i == 0) {
                                    Instantiate(wallPrefabs[0], new Vector2(j, i - 0.15f), Quaternion.identity);
                                    Instantiate(obstacles[GetRandomByRNGCryptoServiceProvider(0, obstacles.Length)], new Vector2(j, i), Quaternion.identity);
                                    break;
                                }
                                //middle
                                Instantiate(obstacles[GetRandomByRNGCryptoServiceProvider(0, obstacles.Length)], new Vector2(j, i), Quaternion.identity);
                                break;
                            }
                        case 'f': {
                                Instantiate(floorPrefabs[GetRandomByRNGCryptoServiceProvider(0, floorPrefabs.Length)], new Vector2(j, i), Quaternion.identity);
                                Instantiate(foodPrefabs[GetRandomByRNGCryptoServiceProvider(0, foodPrefabs.Length)], new Vector2(j, i), Quaternion.identity);
                                foodNum++;
                                break;
                            }
                        case 's': {
                                Instantiate(floorPrefabs[GetRandomByRNGCryptoServiceProvider(0, floorPrefabs.Length)], new Vector2(j, i), Quaternion.identity);
                                Instantiate(headPrefab, new Vector2(j, i), Quaternion.identity);
                                break;
                            }
                        case '.': {
                                Instantiate(floorPrefabs[GetRandomByRNGCryptoServiceProvider(0, floorPrefabs.Length)], new Vector2(j, i), Quaternion.identity);
                                break;
                            }
                    }
                }
            }
        }
    }

    static int GetRandomSeed() {
        byte[] bytes = new byte[4];
        System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
    static int GetRandomByRNGCryptoServiceProvider(int minValue, int maxValue) {
        System.Random random = new System.Random(GetRandomSeed());
        return random.Next(minValue, maxValue);
    }

    // Update is called once per frame
    void Update() {

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
