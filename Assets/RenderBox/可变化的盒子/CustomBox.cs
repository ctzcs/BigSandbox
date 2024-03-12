using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RenderBox.可变化的盒子
{
    public class CustomBox : MonoBehaviour
    {
        public SpriteRenderer sp;
        /// <summary>
        /// 设置成auto tiling模式 就能关联上SpriteRender
        /// </summary>
        public BoxCollider2D box2d;

        private void Awake()
        {
            sp = GetComponent<SpriteRenderer>();
            
        }

        // Start is called before the first frame update
        void Start()
        {
            
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("A");

                var size = sp.size;
                size = new Vector2(size.x + 0.1f,size.y);
                sp.size = size;
            }
        }
    }
}
