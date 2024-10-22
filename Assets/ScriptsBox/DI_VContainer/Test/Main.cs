using System;
using ScriptsBox.DI_VContainer.Test.Battle;
using ScriptsBox.DI_VContainer.Test.State;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ScriptsBox.DI_VContainer.Test
{
    public class Main:MonoBehaviour
    {
        [SerializeField]
        private GameScope _game;

        private LifetimeScope _child;
        public void Start()
        {
            _child = _game.CreateChild<TestBattleScope>();
            Test();
        }

        
        public void Test()
        {
            _game.Container.Resolve<GameData>().PlayerData.PlayerBattleData.id = "1";
            Debug.Log($"{_child.Container.Resolve<PlayerBattleData>().id}");
        }
        
        
        
    }
}