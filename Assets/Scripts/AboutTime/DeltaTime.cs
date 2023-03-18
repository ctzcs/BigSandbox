using UnityEngine;

namespace AboutTime
{
    public class DeltaTime : MonoBehaviour
    {
        // Start is called before the first frame update
        private float elapseTime;
        private float fixedTime;
        void Start()
        {
            Application.targetFrameRate = 120;
            fixedTime = Time.fixedDeltaTime;
        }

        void Update()
        {
            //追帧
            while ((ushort)(elapseTime / fixedTime) > 0)
            {
                FakeUpdate();
                elapseTime -= fixedTime;
            }
            elapseTime += Time.deltaTime;
            print("delta"+Time.deltaTime);
        }

        void FakeUpdate()
        {
            print("Time:" + Time.time);
        }
    } 
}

