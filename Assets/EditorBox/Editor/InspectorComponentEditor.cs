using System;
using UnityEditor;

namespace EditorBox.Editor
{
    [CustomEditor(typeof(InspectorComponent))]
    public class InspectorComponentEditor:UnityEditor.Editor
    {
        private SerializedProperty _inspectorList;
        private void OnEnable()
        {
            Init();
        }

        void Init()
        {
            _inspectorList = serializedObject.FindProperty("InspectorList");
        }
    }
}