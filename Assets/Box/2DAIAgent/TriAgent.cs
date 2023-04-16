using UnityEngine;
using UnityEngine.AI;

namespace _2DAIAgent
{
    public class TriAgent : MonoBehaviour
    {
        public Transform target;
        // Start is called before the first frame update
        void Start()
        {
            /*NavMeshAgent component = GetComponent<NavMeshAgent>();
            component.updateRotation = false;
            component.updateUpAxis = false;
            component.SetDestination(target.position);*/
            //完全不能用在2D
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
