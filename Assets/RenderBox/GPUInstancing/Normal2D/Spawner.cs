using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public GameObject prefab;

    public int count;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                var pos = Random.insideUnitSphere * 10;
                pos.z = 0;
                var obj = Instantiate(prefab);
                obj.transform.position = pos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
