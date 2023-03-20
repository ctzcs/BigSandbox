using System;
using UnityEngine;

namespace AboutTime
{
    public class DeltaTime : MonoBehaviour
    {
        // Start is called before the first frame update
        private float elapseTime;
        private float fixedTime;
        private float accelarateFixedTime;
        void Start()
        {
            Application.targetFrameRate = 120;
            fixedTime = Time.fixedDeltaTime;
        }

        private void FixedUpdate()
        {
            print("fixedTime：" + accelarateFixedTime );
            print("nowTime:" + Time.time);
            accelarateFixedTime += Time.fixedDeltaTime;
        }

        void Update()
        {
            //追帧
            while ((ushort)(elapseTime / fixedTime) > 0)
            {
                FakeUpdate();
                elapseTime -= fixedTime;
            }
            print("delta" + Time.deltaTime);
            elapseTime += Time.deltaTime;
        }

        void FakeUpdate()
        {
            print("InternalUpdate:" + Time.time);
        }
    } 
}

