using UnityEditor;
using UnityEngine;

namespace ScriptsBox.创意工坊
{
    public class CreateGameObject:Editor
    {
        void Save(GameObject gameObject)
        {
            PrefabUtility.SavePrefabAsset(gameObject);
            AssetDatabase.SaveAssets();
        }
    }
}