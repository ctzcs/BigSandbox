using Unity.Entities;
using UnityEngine;

namespace RenderBox.Nsprite.Runtime
{
    public class AnimationSettingAuthoring : MonoBehaviour
    {
        private class AnimationSettingAuthoringBaker : Baker<AnimationSettingAuthoring>
        {
            public override void Bake(AnimationSettingAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new AnimationSetting
                {
                    IdleHash = Animator.StringToHash("Idle"),
                    LeftWalkHash = Animator.StringToHash("Left")
                });
            }
        }
    }


    public struct AnimationSetting : IComponentData
    {
        public int IdleHash;
        public int LeftWalkHash;
    }
}