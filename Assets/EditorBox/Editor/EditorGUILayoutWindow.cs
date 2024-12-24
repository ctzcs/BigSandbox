using System;
using UnityEditor;
using UnityEngine;

namespace EditorBox.Editor
{
    public class EditorGUILayoutWindow:EditorWindow
    {
        [MenuItem("Tools/EditorTutorial/EditorGUILayout")]
        static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<EditorGUILayoutWindow>();
            window.titleContent.text = "编辑器GUI布局实例";
            window.Show();
        }

        private int layer = 0;
        private string tag = "";
        private Color color = Color.clear;
        private ESomeEnum e;
        private ESomeEnum eMulti;
        private int intPopup;
        private bool toggle;
        private bool toggleLeft;
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("文本标题", "文本内容");
            layer =  EditorGUILayout.LayerField("层级选择", layer);
            tag = EditorGUILayout.TagField("标签选择", tag);
            color = EditorGUILayout.ColorField(new GUIContent("标题"),
                color, true,true,true);

            e = (ESomeEnum)EditorGUILayout.EnumPopup("枚举选择", e);
            eMulti = (ESomeEnum) EditorGUILayout.EnumFlagsField("多选枚举",e);
            
            intPopup = EditorGUILayout.IntPopup("整数单选框", intPopup, new string[]{"0","1Hello"}, new int[]{0,1});
            EditorGUILayout.DropdownButton(new GUIContent("按钮上文字"), FocusType.Passive);
            
            
            toggle = EditorGUILayout.Toggle("普通开关", toggle);

            toggleLeft = EditorGUILayout.ToggleLeft("开关在左侧", toggleLeft);
            EditorGUILayout.EndVertical();
        }

        [Flags]
        enum ESomeEnum
        {
            A=1<<0,
            B=1<<1,
            C=1<<2,
        }
    }
}