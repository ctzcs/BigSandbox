using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//结果是并不会响应
//因为啥呢？ action0中只有action1的空指针？
public class DelegateMore : MonoBehaviour
{
    public Action action0;
    public Action action1;
    
    public UnityAction unityAction0;
    public UnityAction unityAction1;
    // Start is called before the first frame update
    void Start()
    {
        
        action0 += action1;
        action1 += LogAction;
        action0?.Invoke();

        unityAction0 += unityAction1;
        unityAction1 += LogUnityAction;
        unityAction0?.Invoke();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            action1.Invoke();
        }
    }

    void LogAction()
    {
        Debug.Log("打印自Action");
    }

    void LogUnityAction()
    {
        Debug.Log("打印自UnityAction");
    }
}
