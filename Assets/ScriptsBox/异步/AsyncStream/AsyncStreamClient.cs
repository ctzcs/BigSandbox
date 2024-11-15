using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Coroutine.AsyncStream
{
    /// <summary>
    /// 通常，yeild是用来生成迭代器
    /// async和await实现异步方法
    /// 流就是一个通道，里面会不停地传输一些东西
    /// </summary>
    public class AsyncStreamClient : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var a = IntAsync();
            Some(a);

        }

        // Update is called once per frame
        void Update()
        {
        
        }

        async Task Some(IAsyncEnumerable<int> a)
        {
            await foreach (var i in a)
            {
                Debug.Log(i);
                Debug.Log(Time.time);
            }
        }
        private 
        async IAsyncEnumerable<int> IntAsync()
        {
            int a = 0;
            for (int i = 0; i < 3; i++)
            {
                a += 1;
                await Task.Delay(1000);
                yield return a;
            }
        }
    }
        
    

}
