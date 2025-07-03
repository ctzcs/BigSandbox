using NSprites;
using Unity.Mathematics;
using UnityEngine;

namespace RenderBox.Nsprite.Runtime
{
    public class SortingGO : MonoBehaviour
    {
        [SerializeField] public int SortingIndex;
        [SortingLayer] [SerializeField] public int SortingLayer;
        // Start is called before the first frame update
        void Start()
        {
            var _sortingGlobalData = SpriteSortingSystem.GetSortingGlobalData();
            var position = transform.position;
            var sortingLayer = UnityEngine.SortingLayer.GetLayerValueFromID(SortingLayer);
            float yOffset = _sortingGlobalData.y *(1 - math.saturate(RemapInternal(position.y, -100, 100, 0, 1)));
            float zValue = sortingLayer * _sortingGlobalData.x
                           + SortingIndex * _sortingGlobalData.y
                           + yOffset; //- _sortingGlobalData.y * math.saturate(RemapInternal(position.y, -100, 100, 0, 1));
            position.z =  -zValue;
            
            
            Debug.Log(_sortingGlobalData +" " + sortingLayer + " " + SortingIndex + zValue);
            transform.position = position;
        }
        
        float RemapInternal(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
    }
}
