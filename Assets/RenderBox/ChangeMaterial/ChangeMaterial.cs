
using UnityEngine;

namespace ChangeMaterial
{
    /// <summary>
    /// 结论：
    /// 1. sharedMaterial改变的时候会改变所有相同材质的颜色
    /// 2. material相当于sharedMaterial的实例化，而且每次用的时候都会实例化。这不好
    /// 3. 如果需要改变mat的值，最好的方式是通过 .CopyPropertiesFromMaterial(otherMaterial)方法来改变现有材质的值。而不是通过已有材质实例化
    /// 4. 尽量使用shadedMaterial,实时替换共享材质，而不是创建实例。其次是Copy其他材质的值，效果也不错。
    /// </summary>
    public class ChangeMaterial : MonoBehaviour
    {
        public Material[] materials;

        private MeshRenderer _renderer;

        public Color color;
    
        // Start is called before the first frame update
        void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material.CopyPropertiesFromMaterial(materials[0]);

        }
        private void OnGUI()
        {
            GUI.BeginGroup(new Rect(100,100,200,300));
            if (GUILayout.Button("白色", GUILayout.Width(200)))
            {
                OnWhitePress();
            }

            if (GUILayout.Button("黑色", GUILayout.Width(200)))
            {
                OnBlackPress();
            }

            if (GUILayout.Button("变色", GUILayout.Width(200)))
            {
                OnValueChange();
            }

            GUI.EndGroup();
        }

        /// <summary>
        /// 替换成白色
        /// </summary>
        void OnWhitePress()
        {
            /*_renderer.sharedMaterial = materials[0];*/
            _renderer.material.CopyPropertiesFromMaterial(materials[0]);
        }

        /// <summary>
        /// 替换成黑色
        /// </summary>
        void OnBlackPress()
        {
        
            _renderer.material.CopyPropertiesFromMaterial(materials[1]);
        }

        /// <summary>
        /// 改变材质的值
        /// </summary>
        void OnValueChange()
        {
            /*//这种方式会直接改变所有的材质
         * _renderer.sharedMaterial.color = color;
         */
            _renderer.material.color = color;
        }
    }
}
