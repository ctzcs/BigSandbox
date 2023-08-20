using UnityEngine;

namespace Box1.ThreeWayCreateBullet
{
    public class WeaponTracer
    {
        /// <summary>
        /// 创建网格
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Mesh CreateMesh(Vector3 startPoint,Vector3 endPoint,float width)
        {
            //这里或许也可以从对象池取方块mesh
            Mesh mesh = new Mesh();
            /*Vector3 pos = (startPoint + endPoint)*0.5f;*/
            //如果要使Mesh创建在GameObject中，则不需要设置位置，mesh中设置位置会导致距离obj中心的偏移。
            float height = Vector3.Distance(startPoint, endPoint);
            float halfHeight = height * .5f;
            float halfWidth = width * .5f;
            Vector3 minPos = new Vector3(- halfWidth ,- halfHeight,0);
            Vector3 maxPos = new Vector3(halfWidth,  halfHeight,0);
            mesh.vertices = new Vector3[4]
            {
                new Vector3(minPos.x,minPos.y,0),
                new Vector3(maxPos.x,minPos.y,0),
                new Vector3(maxPos.x,maxPos.y,0),
                new Vector3(minPos.x,maxPos.y,0)
            };
            mesh.uv = new Vector2[4]
            {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1)
            };
            mesh.triangles = new int[6]
            {
                0,2,1,
                0,3,2
            };
            
            return mesh;
        }
        
        
    }
}