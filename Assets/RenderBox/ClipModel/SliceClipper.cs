using System.Collections.Generic;
using UnityEngine;

namespace Box2.ClipModel
{
    public class SliceClipper : MonoBehaviour
    {
        public GameObject graphic;

        private Material[] _materials;

        private static readonly int SliceNormal = Shader.PropertyToID("_SliceNormal");
        private static readonly int SliceCentre = Shader.PropertyToID("_SliceCentre");

        // Start is called before the first frame update
        void Start()
        {
            _materials = GetMaterials(graphic);
        }

        /// <summary>
        /// 通过设置平面的位置和法线来决定裁剪的平面。
        /// 我经常疑惑，两个物体的shader如何交互，现在看来，可能两个物体之间的shader要交互，就得通过位置和法线关系之类的东西，在脚本里设置另一个shader的参数。
        /// </summary>
        void Update()
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetVector(SliceCentre,transform.position);
                _materials[i].SetVector(SliceNormal,transform.up);
            }
        }
        
        /// <summary>
        /// 获取物体上所有的材质
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        Material[] GetMaterials(GameObject g)
        {
            var renderers = g.GetComponentsInChildren<MeshRenderer>();
            var matList = new List<Material>();
            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    matList.Add(mat);
                }
            }

            return matList.ToArray();
        }
    }
}
