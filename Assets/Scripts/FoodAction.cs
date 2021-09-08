using System.Collections;
using UnityEngine;

public class FoodAction : MonoBehaviour {
    const float high = 0.2f;
    float now = 0;
    private void Start() {
        now = Random.Range(0f, 0.2f);
        transform.Translate(0, now, 0);
        StartCoroutine(move());   
    }
    IEnumerator move() {
        int t = 1;
        float cs = 10f;
        while (true) {
            if (now > high) {
                t = -1;
            }
            if(now < 0) {
                t = 1;
            }
            now += Time.deltaTime * t / cs;
            transform.Translate(0, Time.deltaTime * t / cs, 0);
            yield return 0;
        }
    }
    /*
    private float maxTickTime;
    private float sumTickTime = 0;
    private bool inAction = false;
    private void Start() {
        if (Random.Range(0, 5) == 0) {
            maxTickTime = Random.Range((float)5.0, (float)20.0);
            StartCoroutine(Tick());
            StartCoroutine(Action());
        }
    }

    IEnumerator Tick() {
        while (true) {
            if (inAction) {
                yield return 0;
                continue;
            }
            if (maxTickTime < sumTickTime) {
                sumTickTime = 0;
                inAction = true;
            }
            sumTickTime += Time.deltaTime;
            yield return 0;
        }
    }

    IEnumerator Action() {
        float actionTime = 0;
        while (true) {
            if (inAction) {
                Debug.Log("Start Move");
                for (; actionTime < 0.3;) {
                    yield return 0;
                    actionTime += Time.deltaTime;
                }
                transform.Rotate(0, 0, 30);
                actionTime = 0;
                for (; actionTime < 0.3;) {
                    yield return 0;
                    actionTime += Time.deltaTime;
                }
                actionTime = 0;
                transform.Rotate(0, 0, -30);
                inAction = false;
            }
            yield return 0;
        }
    }
    */
}