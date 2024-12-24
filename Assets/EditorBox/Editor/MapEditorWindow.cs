using UnityEditor;
using UnityEngine;

namespace EditorBox.Editor
{
    public class MapEditorWindow:EditorWindow
    { 
        private bool isEditing = false;
        private GameObject selectedPrefab;
        
        // 添加菜单项
       [MenuItem("Tools/EditorTutorial/MapEditor")]
       static void OpenWindow()
       {
           MapEditorWindow window = GetWindow<MapEditorWindow>("地图编辑器");
           window.Show();
       }
       
        void OnEnable()
       {
           // 订阅场景视图GUI事件
           SceneView.duringSceneGui += OnSceneGUI;
       }
        void OnDisable()
       {
           // 取消订阅
           SceneView.duringSceneGui -= OnSceneGUI;
       }
        void OnSceneGUI(SceneView sceneView)
       {
           Handles.BeginGUI();
           // 编辑器控制面板
           EditorGUILayout.Space(10);
           isEditing = EditorGUILayout.Toggle("启用编辑模式", isEditing);
           
           if (isEditing)
           {
               EditorGUILayout.Space(5);
               selectedPrefab = (GameObject)EditorGUILayout.ObjectField(
                   "要放置的预制体", 
                   selectedPrefab, 
                   typeof(GameObject), 
                   false
               );
               
           }

           var go = Selection.activeGameObject;
           if (go != null)
           {
               EditorGUILayout.Space(10);
               EditorGUILayout.LabelField("预览信息", EditorStyles.boldLabel);
               EditorGUILayout.LabelField("物体名称: " + go.name);

               if (go.TryGetComponent(out SpriteRenderer sr))
               {
                   // 显示预览图
                   float previewSize = 200;
                   Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
                   GUI.DrawTexture(previewRect, sr.sprite.texture, ScaleMode.ScaleToFit);
               }
           }
           
           Handles.EndGUI();
           if (!isEditing || selectedPrefab == null) return;
           Event e = Event.current;
           Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
           RaycastHit hit;
            // 处理点击放置
           if (e.type == EventType.MouseDown && e.button == 0)
           {
               if (UnityEngine.Physics.Raycast(ray, out hit))
               {
                   Vector3 position = SnapToGrid(hit.point);
                   GameObject newObj = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
                   newObj.transform.position = position;
                   
                   Undo.RegisterCreatedObjectUndo(newObj, "Place Object");
                   
                   e.Use();
               }
           }
           
           SceneView.RepaintAll();
       }
        private Vector3 SnapToGrid(Vector3 position)
       {
           float gridSize = 1f;
           return new Vector3(
               Mathf.Round(position.x / gridSize) * gridSize,
               Mathf.Round(position.y / gridSize) * gridSize,
               Mathf.Round(position.z / gridSize) * gridSize
           );
   }
    }
}