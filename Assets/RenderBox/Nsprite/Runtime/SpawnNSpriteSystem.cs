using Unity.Entities;
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
                if (Input.GetMouseButton(0))
                { 
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    
                    Entity e = EntityManager.Instantiate(spawner.ValueRO.Prefab);
                    EntityManager.SetComponentData(e,LocalTransform.FromPosition(pos));
                }
            }
        }
    }
}
