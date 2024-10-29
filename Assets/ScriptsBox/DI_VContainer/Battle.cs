using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace ScriptsBox.DI_VContainer
{
    public class Battle
    {
        private HelloWorldService _helloWorldService;
        private string _id;

        public Battle(HelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }

        public void SetId(string id)
        {
            _id = id;
        }

        [Button]
        public void BattleLoop()
        {
            _helloWorldService.Battle(_id);
        }
    }


    
}