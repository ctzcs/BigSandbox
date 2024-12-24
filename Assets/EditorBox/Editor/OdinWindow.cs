using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorBox.Editor
{
    public class OdinWindow:OdinEditorWindow
    {
        [MenuItem("Tools/EditorTutorial/OdinWindow")]
        static void ShowWindow()
        {
            GetWindow<OdinWindow>();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            SceneView.duringSceneGui += OnSceneGUI;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        

        void OnSceneGUI(SceneView view)
        {
            // 开始Scene GUI
            Handles.BeginGUI();

            // 创建矩形区域
            Rect rect = new Rect(10, 10, 200, 100);
        
            // 使用Odin的GUIBox
            SirenixEditorGUI.BeginBox(new GUIContent("OdinBox"));
            {
                // 标题
                EditorGUILayout.LabelField("Scene Tools");
            
                // 按钮
                if (GUILayout.Button("Do Something"))
                {
                    Debug.Log("Clicked!");
                }
            
                // 滑动条
                //drawer.someValue = EditorGUILayout.Slider("Value", drawer.someValue, 0, 1);
            }
            SirenixEditorGUI.EndBox();

            Handles.EndGUI();
        }
    }
}