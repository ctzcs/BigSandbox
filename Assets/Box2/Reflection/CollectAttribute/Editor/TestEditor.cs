using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Box2.Reflection.CollectAttribute
{
    public class TestEditor : UnityEditor.EditorWindow
    {
        [MenuItem("Tools/ShowWindow")]
        public static void ShowWindow()
        {
            
            var window = EditorWindow.GetWindow<TestEditor>();
            window.Show();
        }
        
        public void CreateGUI()
        {
            var nameList = new List<string>();
            nameList.Add("None");
            //加载程序集，获取所有class
            var types = Assembly.Load("Assembly-CSharp").GetTypes();
            //获取指定特性
            foreach (var currentType in types)
            {
                var attributes = currentType.GetCustomAttributes(typeof(SomeAttribute)).ToArray();
                if (attributes.Length != 0)
                {
                    nameList.Add((attributes[0] as SomeAttribute)?.Name);
                }
            }

            var dropdown = new DropdownField("Magic List : ", nameList, 0, s => s, s =>s);
            rootVisualElement.Add(dropdown);
        }
    }
}
