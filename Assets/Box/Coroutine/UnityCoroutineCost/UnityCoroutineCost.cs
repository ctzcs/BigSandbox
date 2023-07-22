using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Coroutine.UnityCoroutineCost
{
    /// <summary>
    /// 1. 想法：如果要使用Unity内置的协，并且最终需求要用到大量的new WaitForSeconds类似的东西，可以考虑使用对象池存取，这样可以减少gc
    /// 2. 结果：结果一看，这个WaitForSeconds是Native用的密封类，不能手动管理内存，而且就算可以，也没有提供更改里面数值的方法，所以可去你妈的吧。难怪大家都用UniTask了。
    /// 3. 想法2：协程也没比Update好到哪里去，就是Update注意一下阻塞的逻辑就行。量级上去了，要用协程还是异步流的方式用UniTask吧。
    /// </summary>
    public class UnityCoroutineCost : MonoBehaviour
    {
        
        private static readonly WaitForSeconds waitOneSecond = new WaitForSeconds(1);

        public float lastTime = 0;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Wait());
            
        }
        IEnumerator Wait()
        {
            lastTime = Time.time;
            //这里面是个类，必然会在堆上分配内存
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 3; i++)
            {
                //Debug.Log(Time.time - lastTime);
                lastTime = Time.time;
                //这样也是有效的
                yield return waitOneSecond;
            }
        }
    }

    /*public class WaitPool
    {
        private static WaitPool _i;

        public static WaitPool I
        {
            get { return _i ??= new WaitPool(); }
        }

        private Queue<WaitForSeconds> pool = new Queue<WaitForSeconds>();
        private WaitPool(){}

        public void Get(float )
        {
            if (pool.Count <= 0)
            {
                WaitForSeconds w = new WaitForSeconds()
            }
        }
    }*/
}
