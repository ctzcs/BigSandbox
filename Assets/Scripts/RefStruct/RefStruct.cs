using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RefStruct : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        /*unsafe
        {
            int* k = GetPointInt(&i);
            *k += 1;
            Debug.Log(i);
        }*/

        ref int a = ref GetRefInt(ref i);
        a += 1;
        Debug.Log(i);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// it works!
    /// </summary>
    /// <returns></returns>
    unsafe int* GetPointInt(int* a)
    {
        Debug.Log(*a);
        return a;
    }

     /// <summary>
     /// it works!!
     /// </summary>
     /// <returns></returns>
     ref int GetRefInt(ref int a)
     {
         Debug.Log(a);
         return ref a;
     }
    
}
