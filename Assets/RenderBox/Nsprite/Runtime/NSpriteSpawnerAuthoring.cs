using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NSpriteSpawnerSystem : MonoBehaviour
{
    public GameObject Prefab;
    
    class SpawnerBaker : Baker<NSpriteSpawnerSystem>
    {
        public override void Bake(NSpriteSpawnerSystem authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnerNSprite
            {
                // By default, each authoring GameObject turns into an Entity.
                // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.None),
            });
        }
    }
}


public struct SpawnerNSprite : IComponentData
{
    public Entity Prefab;
}