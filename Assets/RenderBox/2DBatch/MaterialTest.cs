using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTest : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Material originalMaterial;
    
    void Start()
    {
        prefab = Resources.Load<GameObject>("Circle2D");
        // 1. 记录预制体的原始材质
        var prefabRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        Debug.Log($"Prefab material: {prefabRenderer.sharedMaterial.name}, ID: {prefabRenderer.sharedMaterial.GetInstanceID()}");
    
        // 2. 实例化对象
        GameObject instance = Instantiate(prefab);
        var instanceRenderer = instance.GetComponentInChildren<SpriteRenderer>();
            
        // 3. 检查实例化后的材质
        Debug.Log($"Instance material: {instanceRenderer.sharedMaterial.name}, ID: {instanceRenderer.sharedMaterial.GetInstanceID()}");
            
        // 4. 比较是否是同一个材质实例
        bool isSameMaterial = prefabRenderer.sharedMaterial == instanceRenderer.sharedMaterial;
        Debug.Log($"Is same material: {isSameMaterial}");
    }
}
