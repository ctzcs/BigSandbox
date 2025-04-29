using System;
using UnityEngine;
using System.Linq.Expressions;

namespace ScriptsBox.ExpressionTree
{
    /// <summary>
    /// 可能对构建dynamic game有用
    /// </summary>
    public class ExpressionTreeDemo : MonoBehaviour
    {
        //目标：构造一个lambda表达式
        //方式：
        //1.参数
        //2.函数体
        //创建一个获取属性的表达式
        public Func<TInstance, TProperty> CreatePropertyGetter<TInstance,TProperty>(string propertyName)
        {
            //lambda参数
            var instance = Expression.Parameter(typeof(TInstance),"instance");
            //lambda函数体->获取实例的某个属性
            var body = Expression.Property(instance,propertyName);
            // 返回的函数应该是
            // TProperty FunctionName(TInstance instance)
            // {
            //        return instance.MemberName;
            // }
            var lambda = Expression.Lambda<Func<TInstance,TProperty>>(body, instance);
            return lambda.Compile();
        } 
        
        //创建一个获取方法的表达式
        public Action<TInstance, object> CreateMethodInvoker<TInstance>(string methodName)
        {
            //参数
            var instance = Expression.Parameter(typeof(TInstance), "instance");
            var arg = Expression.Parameter(typeof(object), "arg");
            var method = typeof(TInstance).GetMethod(methodName) ??
                       throw new ArgumentException($"Method {methodName} not found on type {typeof(TInstance).Name}");
            //body
            var body = Expression.Call(instance, method, Expression.Convert(arg,method.GetParameters()[0].ParameterType));
            //表达式编译
            var lambda = Expression.Lambda<Action<TInstance, object>>(body,instance,arg);
            return lambda.Compile();
        }


        private void Start()
        {
            Player player = new Player();
            //好像构建了一种类似反射的方法，获取值和函数名
            var getPlayerHealth = CreatePropertyGetter<Player, int>("Health");
            var doDamageMethod = CreateMethodInvoker<Player>("DoDamage");
            var health = getPlayerHealth(player);
            doDamageMethod(player,health);
        }
    }


    public class Player
    {
        public int Health { get; set; }
        
        public void DoDamage(){}
    }
}



