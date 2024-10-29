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
        private readonly BattleFactory _battleFactory;
        private IObjectResolver _container;
        public GamePresenter(IObjectResolver container,HelloWorldService helloWorldService,BattleFactory battleFactory)
        {
            _helloWorldService = helloWorldService;
            _helloView = container.Instantiate(Resources.Load<HelloView>("HelloView"));
            _container = container;
            _battleFactory = battleFactory;
        }
        
        void IStartable.Start()
        {
            _helloView.helloBtn.onClick.AddListener(()=> _helloWorldService.Hello());    
        }
        
        void ITickable.Tick()
        {
            //Debug.Log("Tick");

            Battle battle = _battleFactory.Create();
            battle.SetId(battle.GetHashCode().ToString());
            battle.BattleLoop();
        }
    }

    
}