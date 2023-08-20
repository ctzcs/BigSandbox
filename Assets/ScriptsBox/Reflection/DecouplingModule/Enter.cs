
using System;
using UnityEngine;

namespace Box2.Reflection.DecouplingModule
{
    /// <summary>
    /// 基本上所用的方式就是，加载程序集，加载类型，实例化类型
    /// 这种一般是写模块，模块一般都是只有一个
    /// Unity的生命周期，则是每次实例化的时候，都塞一个实例到注册表中
    /// 销毁的时候从注册表中移除
    /// </summary>
    public class Enter:MonoBehaviour
    {
        private void Start()
        {
            //方式一：川哥的方法
            //获取Enter的程序集，遍历继承IModule的脚本
            /*var types = typeof(Enter).Assembly.GetTypes().Where(x => x.GetInterface("IModule") != null);;
            foreach (var type in types)
            {
                //轮询初始化
                (Activator.CreateInstance(type) as IModule)?.Init();
            }*/

            //方式一变体：不用linq
            var types = typeof(Enter).Assembly.GetTypes();
            
            foreach (var type in types)
            {
                if (type.GetInterface("IModule") != null)
                {
                    (Activator.CreateInstance(type) as IModule)?.Init();
                }
            }
        }
        
        
    }

    
}