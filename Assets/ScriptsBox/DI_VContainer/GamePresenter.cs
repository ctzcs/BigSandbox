using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ScriptsBox.DI_VContainer
{
    /// <summary>
    /// 游戏主持人，控制流程
    /// </summary>
    public class GamePresenter:IStartable,ITickable
    {
        private readonly HelloWorldService _helloWorldService;
        private readonly HelloView _helloView;
        
        public GamePresenter(IObjectResolver container,HelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
            _helloView = container.Instantiate(Resources.Load<HelloView>("HelloView"));
        }
        
        void IStartable.Start()
        {
            _helloView.helloBtn.onClick.AddListener(()=> _helloWorldService.Hello());    
        }
        
        void ITickable.Tick()
        {
            //Debug.Log("Tick");
        }
    }

    
}