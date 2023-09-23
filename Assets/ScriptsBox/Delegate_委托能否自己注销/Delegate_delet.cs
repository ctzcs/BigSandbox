using System;
using UnityEngine;
/// <summary>
/// 这种东西的应用场景可能就是，如果我有一张卡，是下一个张牌如果是攻击就触发两次，就可以用这种形式
/// </summary>
public class Delegate_delet : MonoBehaviour
{
    private Action<int> func;
    // Start is called before the first frame update
    void Start()
    {
        func += IsTrue;
        //还在
        func?.Invoke(0);
        //移除
        func?.Invoke(1);
        //func -= IsTrue;
        //空
        func?.Invoke(2);
        
        

    }
    /// <summary>
    /// 如果只打印两句，证明可以在内部删除自己
    ///事实证明是可以的
    /// </summary>
    /// <param name="input"></param>
    void IsTrue(int input)
    {
        if (input == 1)
        {
            //如果触发了这个条件就删除
            func -= IsTrue;
            Debug.Log("应该被删除了");
        }
        else
        {
            Debug.Log("我还在");  
        }
        
    }
}
