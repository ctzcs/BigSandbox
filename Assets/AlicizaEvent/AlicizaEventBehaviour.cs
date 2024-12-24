using System;
using System.Diagnostics;
using AlicizaFramework;
using UnityEngine;

namespace AlicizaFramework
{
    public partial interface ITestLoginEvent : IEvent
    {
        public string Account { get; set; }
        public string password { get; set; }
    }

    public partial interface IAppLaunchEvent : IEvent
    {
    }
}


public class AlicizaEventBehaviour : MonoBehaviour
{
    private void Start()
    {
        TestEvent();
        NoParamTestEvent();
    }

    public int count;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#endif

    public void TestEvent()
    {
        EventPublisher _publisher = new EventPublisher();
        _publisher.Subscribe<ITestLoginEvent>(OnLoginEvent);
        _publisher.Subscribe<IAppLaunchEvent>(AppLaunchEvent);


        const int iterations = 10000000; // 一千万次
        Stopwatch stopwatch = new Stopwatch();

        // 预热：为了排除JIT编译的影响，先调用一次测试方法
        _publisher.Send<ITestLoginEvent>(account: "测试", password: "adda");

        stopwatch.Start();

        for (int i = 0; i < iterations; i++)
        {
            _publisher.Send<ITestLoginEvent>(account: "测试", password: "adda");
        }

        stopwatch.Stop();

        UnityEngine.Debug.Log($" Has Param Total Time: {stopwatch.ElapsedMilliseconds} ms");
        UnityEngine.Debug.Log($"Average Time per Call: {(double)stopwatch.ElapsedMilliseconds / iterations} ms");
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#endif

    public void NoParamTestEvent()
    {
        EventPublisher _publisher = new EventPublisher();
        _publisher.Subscribe<ITestLoginEvent>(OnLoginEvent);
        _publisher.Subscribe<IAppLaunchEvent>(AppLaunchEvent);


        const int iterations = 10000000; // 一千万次
        Stopwatch stopwatch = new Stopwatch();
        _publisher.Send<IAppLaunchEvent>();

        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            _publisher.Send<IAppLaunchEvent>();
        }
        stopwatch.Stop();

        UnityEngine.Debug.Log($" No Param Total Time: {stopwatch.ElapsedMilliseconds} ms");
        UnityEngine.Debug.Log($"Average Time per Call: {(double)stopwatch.ElapsedMilliseconds / iterations} ms");
    }


    private void AppLaunchEvent()
    {
        // Debug.Log("Launch");
        count++;
        count += 16;
        count += 16;
    }

    private void OnLoginEvent(string account, string password)
    {
        // Debug.Log("111");
        count++;
    }
}
