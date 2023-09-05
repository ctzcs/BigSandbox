using UnityEngine;

public class InstanceIdTest : MonoBehaviour
{
    public int[] id;
    // Start is called before the first frame update
    void Start()
    {
        id = new int[10];
        id[0] = this.gameObject.GetInstanceID();
        id[1] = GetComponent<Rigidbody>().GetInstanceID();
        id[2] = GetComponent<Collider>().GetInstanceID();
        id[3] = transform.GetInstanceID();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
