using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using UnityEngine;
using VContainer;

namespace ScriptsBox.DI_VContainer.Test.Battle
{
    public class Scene
    {
        private PlayerBattleData _data;
        public Scene(PlayerBattleData data)
        {
            _data = data;
        }
    }
}