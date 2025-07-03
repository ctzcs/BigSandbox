using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace NSprites
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    public partial struct SpriteSortingSystem : ISystem
    {
        private static readonly int SortingGlobalData = Shader.PropertyToID("_sortingGlobalData");
        private static readonly int Bound = Shader.PropertyToID("_bound");
        public const int LayerCount = 5;
        public const int SortingIndexCount = 5;

        private const float PerLayerOffset = 1f / LayerCount;
        private const float PerSortingIndexOffset = PerLayerOffset / SortingIndexCount;

        public void OnCreate(ref SystemState state)
        {
            Shader.SetGlobalVector(SortingGlobalData, new Vector4(PerLayerOffset, PerSortingIndexOffset, default, default));
            Shader.SetGlobalVector(Bound, new Vector4(-100, 100, -100, 100));
        }
            
        
        public static float4 GetSortingGlobalData()=>
            new Vector4(PerLayerOffset, PerSortingIndexOffset, default, default);
    }
}

