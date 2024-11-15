using System;
using UnityEngine;

namespace UniTaskTutorial.BaseUsing.Scripts
{
    [Serializable]
    public class Runner
    {
        public Transform Target;
        public float Speed = 5f;
        public Vector3 StartPos;
        public bool ReachGoal = false;

        public void Reset()
        {
            ReachGoal = false;
            Target.position = StartPos;
        }
        

    }
}