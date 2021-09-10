using System.Collections.Generic;
using UnityEngine;

namespace FallBack {

    public struct StateSave {
        public bool ate;
        public Snake body;
        public Vector2 dir;
    }

    public class FallBackManager {
        static public FallBackManager Instace = new FallBackManager();
        private Stack<StateSave> stack = new Stack<StateSave>();
        private FallBackManager() {

        }
        public void resetState() {
            //Debug.Log("Clear FallBackStack");
            stack.Clear();
        }
        public void addState(StateSave saver) {
            stack.Push(saver);
        }
        public void FallBack() {
            //Debug.Log("Start FallBack");
            if (stack.Count == 0) return;
            StateSave saver = stack.Pop();
            Snake snakePtr = saver.body;
            if (saver.ate) {
                snakePtr = saver.body.bodyInFront;
                saver.dir = -saver.body.dir;
                GameObject.Destroy(saver.body.gameObject);
                snakePtr.bodyInBack = null;
            }
            while (snakePtr != null) {
                saver.dir = snakePtr.FallBack(saver.dir, saver.ate);
                snakePtr = snakePtr.bodyInFront;
            }
        }

    }

}


