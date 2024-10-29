using System;
using System.Collections.Generic;
using System.Threading;

namespace ScriptsBox.ControllerTreeArchitecture
{
    public interface IController { }
    public interface IControllerFactory{}

    class CompositeDisposable
    {
        
    }
    class ControllerState
    {
        //开始
        //暂停
        //结束
    }

    public abstract partial class ControllerBase:IController,IDisposable
    {
        private readonly List<IController> _childController; //收集所有的子对象
        private readonly CompositeDisposable _compositeDisposables = new();//应该是所有的要Dispose的子对象
        private readonly IControllerFactory _controllerFactory;
        private CancellationToken _lifetimeToken;
        private CancellationTokenSource _lifetimeTokenSource;
        private ControllerState _state;
        protected void Add(IController controller){_childController.Add(controller);}
        public abstract void Dispose();
    }
    
}