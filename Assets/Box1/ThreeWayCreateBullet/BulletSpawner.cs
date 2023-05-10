using System;
using UnityEngine;

namespace Box1.ThreeWayCreateBullet
{
    public class BulletSpawner:MonoBehaviour
    {
        private WeaponTracer _weaponTracer;
        public Material mat;
        public Transform myTransform;
        private void Awake()
        {
            myTransform = this.transform;
            _weaponTracer = new WeaponTracer();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var pos = Input.mousePosition;
                pos.z = 10;
                Vector3 endPos = Camera.main.ScreenToWorldPoint(pos);
                /*Debug.Log($"{endPos}:{myTransform.position}");*/
                CreateBulletTracer(myTransform.position, endPos, 0.05f);
            }
        }
        
        /// <summary>
        /// 程序化网格生成Tracer的方法
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        GameObject CreateBulletTracer(Vector3 startPos,Vector3 endPos,float width)
        {
            Mesh mesh = _weaponTracer.CreateMesh(startPos, endPos,width);
            GameObject obj = new GameObject("Tracer");//其实这里应该从对象池取出来的
            MeshRenderer rd = obj.AddComponent<MeshRenderer>();
            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            rd.material = mat;
            var dir = (endPos - startPos).normalized;
            /*var angle = Vector3.Angle(Vector3.up, dir);*///这个由于得出的总是正值，所以在360的情况下有问题。
            var rotate = Quaternion.FromToRotation(Vector3.up, dir);
            obj.transform.position = (startPos + endPos) * .5f;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localRotation *= rotate;
            return obj;
        }

    }

    
}