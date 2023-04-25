using UnityEngine;

/// <summary>
/// 进入相机的时候开启逻辑
/// </summary>
public class IntoCamera : MonoBehaviour
{
    private int _frame;
    private void OnBecameVisible()
    {
        enabled = true;
        
        Debug.Log("相机可见");    
    }

    private void OnBecameInvisible()
    {
        enabled = false;
        Debug.Log("相机不可见");
    }

    private void Update()
    {
        _frame =(int)(1 / Time.deltaTime) ;
        print(_frame);
    }
}
