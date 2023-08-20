
using System;
using System.Buffers;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
///目标是给出一个起点，一个终点，创建一个方形的弹道
/// </summary>
public class CodeMesh : MonoBehaviour
{
    public Material mat;
    private Mesh m;
    private float lastH;
    public float h;
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = new GameObject("quad");
        MeshRenderer rd = obj.AddComponent<MeshRenderer>();
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        m = CreateMesh(Vector3.zero, new Vector3(1,4,0), quaternion.identity);
        filter.mesh = m;
        rd.sharedMaterial = mat;
    }

    private void Update()
    {
        
        if (Math.Abs(h - lastH) > 0.01f)
        {
            ChangeHeight(m,h);
            lastH = h;
        }
    }

    /// <summary>
    /// 创建mesh
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="size">大小</param>
    /// <param name="quaternion">旋转</param>
    /// <returns></returns>
    Mesh CreateMesh(Vector3 pos,Vector3 size,Quaternion quaternion)
    {
        Mesh mesh = new Mesh();
        //锚点在底部中间
        Vector3 minPos = new Vector3(pos.x - size.x *0.5f,pos.y,0);
        Vector3 maxPos = new Vector3(pos.x + size.x * 0.5f, pos.y + size.y,0);
        mesh.vertices = new Vector3[4]
        {
            new Vector3(minPos.x,minPos.y),
            new Vector3(maxPos.x,minPos.y),
            new Vector3(maxPos.x,maxPos.y),
            new Vector3(minPos.x,maxPos.y)
        };
        //保证是顺时针的顺序，就能在外面
        mesh.triangles = new int[6]
        {
            0,2,1,
            0,3,2
        };
        mesh.uv = new Vector2[4]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1)
        };
        return mesh;
    }

    void ChangeHeight(Mesh m,float h)
    {
        Vector3 pos = new Vector3((m.vertices[0].x+ m.vertices[1].x)*0.5f,m.vertices[0].y,0);
        Vector3 size =new Vector3((m.vertices[1].x - m.vertices[0].x), m.vertices[2].y - m.vertices[1].y, 0);
        size.y += h;
        Vector3 minPos = new Vector3(pos.x - size.x *0.5f,pos.y,0);
        Vector3 maxPos = new Vector3(pos.x + size.x * 0.5f, pos.y + size.y,0);
        m.vertices = new Vector3[4]
        {
            new Vector3(minPos.x,minPos.y),
            new Vector3(maxPos.x,minPos.y),
            new Vector3(maxPos.x,maxPos.y),
            new Vector3(minPos.x,maxPos.y)
        };
    }
    
}
