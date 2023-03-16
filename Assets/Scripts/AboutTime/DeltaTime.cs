using UnityEngine;

namespace AboutTime
{
    public class DeltaTime : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void FixedUpdate()
        {
            Debug.Log("fixed:" + Time.fixedDeltaTime);
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("update:" + Time.deltaTime);
        }
    } 
}

