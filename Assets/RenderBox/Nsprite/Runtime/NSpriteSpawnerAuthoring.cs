using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NSpriteSpawnerSystem : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Prefab2;
    public GameObject SpriteRender;
    
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
                Prefab2 = GetEntity(authoring.Prefab2, TransformUsageFlags.None),
                SpriteRender = GetEntity(authoring.SpriteRender, TransformUsageFlags.None)
            });
        }
    }
}


public struct SpawnerNSprite : IComponentData
{
    public Entity Prefab;
    public Entity Prefab2;
    public Entity SpriteRender;
}