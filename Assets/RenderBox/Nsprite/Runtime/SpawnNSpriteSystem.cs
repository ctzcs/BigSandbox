using System.IO;
using NSprites;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Transforms;
using UnityEngine;


namespace RenderBox.Nsprite.Runtime
{
    public partial class SpawnNSpriteSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            foreach (var spawner in SystemAPI.Query<RefRW<SpawnerNSprite>>())
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                if (Input.GetKey(KeyCode.Z))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Entity e = EntityManager.Instantiate(spawner.ValueRO.SpriteRender);
                        EntityManager.SetComponentData(e,LocalTransform.FromPosition(pos));
                    }
                }
                
                else if (Input.GetMouseButton(0))
                { 
                    Entity e = EntityManager.Instantiate(spawner.ValueRO.Prefab);
                    EntityManager.SetComponentData(e,LocalTransform.FromPosition(pos));
                }

                else if (Input.GetMouseButton(1))
                {
                    Entity e = EntityManager.Instantiate(spawner.ValueRO.Prefab2);
                    EntityManager.SetComponentData(e,LocalTransform.FromPosition(pos));
                }

                else if (Input.GetMouseButtonDown(2))
                {
                    if (SystemAPI.TryGetSingleton<AnimationSetting>(out var setting))
                    {
                        foreach (var animator in SystemAPI.Query<AnimatorAspect>())
                        {
                            if (animator.GetNowAnimationIndex != 1)
                            {
                                animator.SetAnimation(setting.LeftWalkHash, 0);
                            }
                            else
                                animator.SetAnimation(setting.IdleHash, 0);
                        }
                    }
                }

                
            }
        }
    }
}
