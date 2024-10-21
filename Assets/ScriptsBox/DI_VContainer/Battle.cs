using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace ScriptsBox.DI_VContainer
{
    public class Battle
    {
        private HelloWorldService _helloWorldService;

        [Inject]
        public void Constructor(HelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }

        [Button]
        void BattleLoop()
        {
            _helloWorldService.Battle();
        }
    }
}