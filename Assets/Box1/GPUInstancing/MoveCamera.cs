using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Box1.GPUInstancing
{
    public class MoveCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(horizontal, 0, vertical);
            transform.position += dir.normalized * Time.deltaTime;

        }
    }
}
