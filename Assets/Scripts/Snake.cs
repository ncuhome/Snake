using FallBack;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public enum BodyFace {
    Up = 0,
    Right,
    Down,
    Left
}

public class Snake : MonoBehaviour {
    public Snake bodyInFront;
    public Snake bodyInBack;
    private BodyFace bodyFace;
    public SpriteRenderer spriteRenderer;
    public Sprite Head;
    public Sprite Body1;
    public Sprite Body2;
    public Sprite Body3;
    public Sprite Tail;
    public bool bodyIsHead { get { return bodyInFront == null; } }
    public bool bodyIsTail { get { return bodyInBack == null; } }
    public Vector2 dir;
    static Vector2[] faceInVec2 = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    static Vector3[] faceInVec3 = { new Vector3(0, 0, 0), new Vector3(0, 0, 270), new Vector3(0, 0, 180), new Vector3(0, 0, 90) };
    private bool ate = false;
    private bool canFallBack = false;
    public bool CanFallBack { get => canFallBack; }

    private int score = 0;

    //运行时间，以1为起始,用于时间加速
    // private float runningTime = 1f;

    public AudioClip eatClip;

    public GameObject foodPrefab;
    //时间加速单位
    // public double boostScale = 1.1f;

    public GameObject tailPrefab;
    // Start is called before the first frame update

    //手机滑屏触发输入距离的平方
    private float minDistance = Screen.width * 0.1f;
    // private Vector2 tailEndPos;

    private Vector2 lastDir;

    // private bool lastAte;

    private bool moved;
    // private Coroutine monsterCanBeEatenCoroutine;

    private static Snake instance;
    public static Snake Instance {
        get { return instance; }
    }
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        bodyFace = BodyFace.Up;
    }
    void Start() {
        Input.simulateMouseWithTouches = true;
        Input.multiTouchEnabled = true;
    }


    //等待优化
    void Update() {
        if (!GlobalManager.Instance.getPaused() && bodyIsHead) {
            //一次输入后，在移动之前，使输入无效化，防止出现同时按下两个键，在移动判断之前导致dir变换两个方向，导致自己撞自己的bug产生而死亡

            #region
            //桌面端输入控制
            if (Input.GetKeyDown(KeyCode.RightArrow) && (dir != -Vector2.right) && canMove(Vector2.right)) {
                lastDir = dir;
                dir = Vector2.right;
                Move();
            } else if (Input.GetKeyDown(KeyCode.DownArrow) && (dir != Vector2.up) && canMove(-Vector2.up)) {
                lastDir = dir;
                dir = -Vector2.up;
                Move();
            } else if (Input.GetKeyDown(KeyCode.LeftArrow) && (dir != Vector2.right) && canMove(-Vector2.right)) {
                lastDir = dir;
                dir = -Vector2.right;
                Move();
            } else if (Input.GetKeyDown(KeyCode.UpArrow) && (dir != -Vector2.up) && canMove(Vector2.up)) {
                lastDir = dir;
                dir = Vector2.up;
                Move();
            }
            #endregion
            #region
            //手机端输入控制
            //滑屏控制
            if (!GlobalManager.Instance.getIsJoyStick()) {
                if (!moved && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
                    if (Vector2.SqrMagnitude(Input.GetTouch(0).deltaPosition) > minDistance) {
                        Vector2 deltaDir = Input.GetTouch(0).deltaPosition;
                        if (Mathf.Abs(deltaDir.x) > Mathf.Abs(deltaDir.y)) {
                            if (deltaDir.x > 0 && (dir != -Vector2.right) && canMove(Vector2.right)) {
                                lastDir = dir;
                                dir = Vector2.right;
                                Move();
                            }
                            if (deltaDir.x < 0 && (dir != Vector2.right) && canMove(-Vector2.right)) {
                                lastDir = dir;
                                dir = -Vector2.right;
                                Move();
                            }
                        }
                        if (Mathf.Abs(deltaDir.y) > Mathf.Abs(deltaDir.x)) {
                            if (deltaDir.y > 0 && (dir != -Vector2.up) && canMove(Vector2.up)) {
                                lastDir = dir;
                                dir = Vector2.up;
                                Move();
                            }
                            if (deltaDir.y < 0 && (dir != Vector2.up) && canMove(-Vector2.up)) {
                                lastDir = dir;
                                dir = -Vector2.up;
                                Move();
                            }
                        }
                    }
                    moved = true;
                }
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
                    moved = false;
                }
                #endregion
            }
            // runningTime += Time.deltaTime;
            // Time.timeScale = (float)Pow(boostScale, Log(runningTime));
        }
        //ExchangeSprite();
    }
    #region 
    //虚拟按键控制,放在这方便控制方向变量
    //但是挂载函数放在了globalManager
    public void onClickLeft() {
        if (canMove(-Vector2.right) && (dir != Vector2.right)) {
            lastDir = dir;
            dir = -Vector2.right;
            Move();
        }
    }
    public void onClickRight() {
        if (canMove(Vector2.right) && (dir != -Vector2.right)) {
            lastDir = dir;
            dir = Vector2.right;
            Move();
        }
    }
    public void onClickUp() {
        if (canMove(Vector2.up) && (dir != -Vector2.up)) {
            lastDir = dir;
            dir = Vector2.up;
            Move();
        }
    }
    public void onClickDown() {
        if (canMove(-Vector2.up) && (dir != Vector2.up)) {
            lastDir = dir;
            dir = -Vector2.up;
            Move();
        }
    }
    #endregion
    /*
    void OnTriggerEnter2D(Collider2D other) {
        if (other.name.StartsWith("FoodPrefab")) {
            ate = true;
            AudioSource.PlayClipAtPoint(eatClip, Camera.main.transform.position);
            Destroy(other.gameObject);
            GlobalManager.Instance.updateScore(score);
        }
    }
    */

    void ExchangeSprite() {
        for (int iface = 0; iface < 4; iface++) {
            if (dir == faceInVec2[iface]) {
                bodyFace = (BodyFace)iface;
                transform.eulerAngles = faceInVec3[iface];
            }
        }
        ///*
        if (bodyIsHead) {
            if (spriteRenderer.sprite != Head) spriteRenderer.sprite = Head;
            return;
        }
        if (!bodyIsTail) {
            int ft = bodyFace - bodyInBack.bodyFace + 4;
            ft %= 4;
            //Debug.Log(ft);
            switch (ft) {
                case 0:
                    if (spriteRenderer.sprite != Body1) spriteRenderer.sprite = Body1;
                    break;
                case 2:
                    if (spriteRenderer.sprite != Body1) spriteRenderer.sprite = Body1;
                    break;
                case 1:
                    if (spriteRenderer.sprite != Body2) spriteRenderer.sprite = Body2;
                    break;
                case 3:
                    if (spriteRenderer.sprite != Body3) spriteRenderer.sprite = Body3;
                    break;
            }
        } else {
            if (spriteRenderer.sprite != Tail) spriteRenderer.sprite = Tail;
        }
        //*/
    }

    private bool isEatFood() {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        if (hit.collider != null && hit.collider.name.StartsWith("FoodPrefab")) {
            ate = true;
            AudioSource.PlayClipAtPoint(eatClip, Camera.main.transform.position);
            Destroy(hit.collider.gameObject);
            return true;
        }
        return false;
    }

    Snake cloneNewBody() {
        GameObject newBody = Instantiate(tailPrefab, transform.position, transform.rotation);
        newBody.name = "Tail";
        Snake newBodyScript = newBody.GetComponent<Snake>();
        newBodyScript.bodyInFront = this;
        newBodyScript.bodyInBack = bodyInBack;
        bodyInBack = newBodyScript;
        bodyInBack.dir = Vector2.zero;
        return newBodyScript;
    }

    bool Move(bool isEat = false) {
        //
        bool SpecialEat = false;
        if (bodyIsHead && isEatFood()) {
            isEat = true;
            SpecialEat = true;
        }
        if (bodyIsTail && isEat) {
            cloneNewBody();
            isEat = false;
        }
        transform.position += (Vector3)dir;
        if (bodyIsHead) {
            StateSave saver = new StateSave();
            saver.ate = SpecialEat;
            saver.dir = -dir;
            saver.body = this;
            Snake snakePtr = bodyInBack;
            while (snakePtr != null) {
                saver.body = snakePtr;
                saver.dir = -snakePtr.dir;
                isEat = snakePtr.Move(isEat);
                snakePtr = snakePtr.bodyInBack;
            }
            FallBackManager.Instace.addState(saver);
            snakePtr = saver.body;
            while (snakePtr != null) {
                snakePtr.ExchangeSprite();
                snakePtr = snakePtr.bodyInFront;
            }
        } else {
            dir = bodyInFront.transform.position - transform.position;
        }
        //ExchangeSprite();
        return isEat;
        #region
        /*
        StatsSave cache = new StatsSave();
        cache.dir = lastDir;
        cache.ate = ate;
        if (tail.Count > 0) {
            cache.tailEndPosition = tail.Last().position;
        } else {
            cache.tailEndPosition = Vector2.zero;
        }
        if (fallbackCaches.Count >= 10) {
            fallbackCaches.RemoveAt(0);
            fallbackCaches.Add(cache);
        } else {
            fallbackCaches.Add(cache);
            canFallBack = true;
        }

        Vector2 v = transform.position;

        transform.Translate(dir);
        if (ate) {
            GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);
            tail.Insert(0, g.transform);
            ate = false;
        } else if (tail.Count > 0) {
            tail.Last().position = v;
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }
        */
        #endregion
    }

    public Vector2 FallBack(Vector2 saverDir, bool isEat) {
        //Debug.Log("fallback");
        if (bodyIsHead && isEat) {
            Instantiate(foodPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        }
        transform.position += (Vector3)saverDir;
        Vector2 nxtDir = -dir;
        if (bodyIsHead && !bodyIsTail) dir = bodyInBack.dir;
        else dir = -saverDir;
        ExchangeSprite();
        return nxtDir;
        #region
        /*
        var cache = fallbackCaches.Last();
        fallbackCaches.RemoveAt(fallbackCaches.Count - 1);
        if (fallbackCaches.Count == 0) canFallBack = false;
        var lastAte = cache.ate;
        var tailEndPos = cache.tailEndPosition;
        var lastDirection = cache.dir;
        if (lastAte) {
            //连续吃两个食物
            if (ate) {
                Vector2 headPos = transform.position;
                var tailObj = tail.First().gameObject;
                tail.RemoveAt(0);
                Destroy(tailObj);
                transform.Translate(-dir);
                dir = lastDirection;
                Instantiate(foodPrefab, headPos, Quaternion.identity);
            }
            //只吃了一个食物
            else {
                ate = true;
                var tailObj = tail.First().gameObject;
                tail.RemoveAt(0);
                Destroy(tailObj);
                transform.Translate(-dir);
                dir = lastDirection;
            }
        } else if (tail.Count > 0) {
            if (ate) {
                ate = lastAte;
                var tailFirst = tail.First();
                Vector2 headPos = transform.position;
                tailFirst.position = tailEndPos;
                tail.Add(tailFirst);
                tail.RemoveAt(0);
                transform.Translate(-dir);
                dir = lastDirection;
                Instantiate(foodPrefab, headPos, Quaternion.identity);
            } else {
                var tailFirst = tail.First();
                tailFirst.position = tailEndPos;
                tail.Add(tailFirst);
                tail.RemoveAt(0);
                transform.Translate(-dir);
                dir = lastDirection;
            }
        } else {
            if (ate) {
                ate = lastAte;
                Vector2 headPos = transform.position;
                transform.Translate(-dir);
                dir = lastDirection;
                Instantiate(foodPrefab, headPos, Quaternion.identity);
            } else {
                transform.Translate(-dir);
                dir = lastDirection;
            }
        }
        // isFallBack = true;
        */
        #endregion
    }

    private bool canMove(Vector2 dir) {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir / 2, pos + dir);
        if (hit.collider == null) return true;
        if (hit.collider.name.StartsWith("Tail")) return false;
        if (hit.collider.gameObject.tag == "wall") return false;
        return true;
    }
}
