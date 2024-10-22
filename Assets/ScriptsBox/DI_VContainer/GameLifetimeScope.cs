using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ScriptsBox.DI_VContainer
{
    /// <summary>
    /// 这是一个容器的作用域，可能会创建和销毁
    /// </summary>
    public class GameLifetimeScope:LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            //RegisterEntryPointer是用于注册与Unity的PlayerLoop事件相关接口的别名，不依赖Mono注册生命周期，有助于解耦领域逻辑与表现层
            builder.RegisterEntryPoint<GamePresenter>();
            //注册逻辑
            builder.Register<HelloWorldService>(Lifetime.Singleton);
            //注册显示
            //builder.RegisterComponent(_helloView);

            builder.Register<Battle>(Lifetime.Singleton);

            /*
            //如果有多个入口点，也可以这样
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
            {
                entryPoints.Add<GamePresenter>();
                // entryPoints.Add<OtherSingletonEntryPointA>();
                // entryPoints.Add<OtherSingletonEntryPointB>();
                // entryPoints.Add<OtherSingletonEntryPointC>();
            });
            */
        }
    }
}