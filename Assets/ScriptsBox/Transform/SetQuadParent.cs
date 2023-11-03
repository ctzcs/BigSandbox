using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuadParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        obj.transform.position = new Vector3(0, 0, 100);
        
        var obj1 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        obj1.transform.position = new Vector3(0, 0, 99);
        obj1.transform.SetParent(obj.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
