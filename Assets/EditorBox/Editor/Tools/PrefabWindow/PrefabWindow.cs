
using UnityEditor;
using UnityEngine;

public class PrefabWindow : EditorWindow
{

    [MenuItem("Tools/Utils/PrefabWindow")]
    static void Open()
    {
        var window = GetWindow<PrefabWindow>();
        window.Show();
    }

    [SerializeField] private GameObject gameObject;
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.BeginVertical();
        gameObject = EditorGUILayout.ObjectField("预制体",gameObject,typeof(GameObject),true) as GameObject;
        if (GUILayout.Button(new GUIContent("保存")))
        {
            Save();
        }
        EditorGUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    void Save()
    {
        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
        if (prefab is null)
        {
            Debug.Log("获取不到预制体");
            return;
        }
        if (PrefabUtility.IsPartOfPrefabInstance(prefab) || PrefabUtility.IsPartOfAnyPrefab(prefab))
        {
            // 获取预制体资源路径
            
            // 应用所有修改
            PrefabUtility.ApplyPrefabInstance(gameObject, InteractionMode.UserAction);
            // 保存修改
            
            PrefabUtility.SavePrefabAsset(prefab);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"{prefab}不是预制体实例");
        }
    }
}
