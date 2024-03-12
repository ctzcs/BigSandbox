
using System.Collections.Generic;
using UnityEngine;

public class Delete : MonoBehaviour
{
    List<Cool> cools = new();
    int tick = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        var obj = new GameObject();
        var cool = obj.AddComponent<Cool>();
        cools.Add(cool);
        GameObject.DestroyImmediate(obj);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        tick++;
        if (tick > 10)
        {
            //Debug.Log($"cools length{cools.Count}\n Is cool[0] null {cools[0]}");
        }
    }
}
public interface ICool
{
        
}

public class Cool:MonoBehaviour,ICool{}