using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class BaseAbility : MonoBehaviour
{

    public enum EAbilityType
    {
        
    }
    
    [ShowInInspector,OdinSerialize]
    public List<BuffConfig> _buffs = new(); 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public class BuffConfig : SerializedScriptableObject
{
    
}

[ShowInInspector]
[CreateAssetMenu(fileName = "Frozen",menuName = "Config/Frozen")]
public class FrozenBuffConfig : BuffConfig
{
    [ShowInInspector]
    public int a;
    [ShowInInspector]
    public int b;
}
