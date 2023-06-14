using System;

using System.Collections.Generic;

using Clipper2Lib;

using Poly2Tri;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Box1.YouCutMe
{
    public enum EShape
    {
        Red,
        White
    }
    public class Shape : MonoBehaviour
    {
        
        #region public
        
        public MeshFilter Filter => _filter;
        public MeshRenderer Renderer => _renderer;
        public Transform Trans => transform;
        #endregion
        private MeshFilter _filter;
        private MeshRenderer _renderer;
        [SerializeField]
        private EShape shape;
        private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock _block;
        
        
        // Start is called before the first frame update
        void Start()
        {
            
            Init();
            //这里也说明我们应该提前加载配置，然后再进入逻辑.
            if (shape == EShape.Red)
            {
                SetColor(new Color32(255,0,0,255));   
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var info = Physics2D.Raycast(point, Vector3.zero);
                
                if (info.transform != this.transform)
                {
                    Shape s;
                    bool success = info.transform.TryGetComponent(out s);
                    if (success)
                    {
                        Debug.Log(shape + ":Successful!");
                        /*CutBy(s);*/
                        PathsD pathsD = Difference(this,s);
                        this.Filter.mesh = PolygonToMesh(pathsD, this); //PathDToMesh(pathsD, this);
                    }
                     
                }
                
            }
        }

        void Init()
        {
            TryGetComponent(out _renderer);
            TryGetComponent(out _filter);
            _block = new MaterialPropertyBlock();
        }
        
        void SetShape(EShape shape)
        {
            this.shape = shape;
        }

        void SetColor(Color32 color)
        {
            _block.SetColor(Color1,color);
            _renderer.SetPropertyBlock(_block);
        }

        public void CutBy(Shape other)
        {
            Dictionary<int,Vector3> pointsInOther = new Dictionary<int, Vector3>();
            //1. 找本图形到在另外一个shape中的点，存到list中
            Vector3[] vertices = _filter.mesh.vertices;
            List<Vector3> sortVertices = SortRound(vertices);
            for (int i = 0; i < sortVertices.Count; i++)
            {
                var worldPoint = transform.TransformPoint(vertices[i]);
                worldPoint.z = 0;
                if (ContainsPoint(worldPoint,other))
                {
                    Debug.Log($"找到一个{worldPoint}");
                    pointsInOther.Add(i,worldPoint);
                }
            }
            
            //2. 从点发射射线，与另外一个形状求交点
            List<Vector3> points = new List<Vector3>();
            
            foreach (KeyValuePair<int,Vector3> pair in pointsInOther)
            {
                Debug.Log($"在形状内{pair.Value}");
                int pre = pair.Key == 0 ? sortVertices.Count - 1:pair.Key - 1;
                int next = pair.Key == sortVertices.Count - 1 ? 0:pair.Key + 1;
                
                RaycastHit2D[] info1 = new RaycastHit2D[10];
                RaycastHit2D[] info2 = new RaycastHit2D[10];
                int num1 = Physics2D.LinecastNonAlloc(sortVertices[pre],pair.Value ,info1);
                for (int i = 0; i < num1; i++)
                {
                    points.Add(info1[i].point);
                }
                int num2 = Physics2D.LinecastNonAlloc(sortVertices[next],pair.Value, info2);
                for (int i = 0; i < num2; i++)
                {
                    points.Add(info2[i].point);
                }
            }
            
            for (int i = 0; i < points.Count; i++)
            {
                Debug.Log($"交点{points[i]}");
            }
        }

        /// <summary>
        /// 传入世界坐标系的点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static bool ContainsPoint(Vector3 point,Shape s)
        {
            Mesh mesh = s.Filter.mesh;
            Vector3 localPoint = s.Trans.InverseTransformPoint(point);
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].z = 0;
            }

            List<Vector3> verticesList =  SortRound(vertices);
            
            for (int i = 0; i < verticesList.Count ; i++)
            {
                int next = i + 1;
                if (i == verticesList.Count - 1 )
                {
                    next = 0;
                }
                Vector3 line = verticesList[next] - verticesList[i];
                Vector3 line2 = localPoint - verticesList[i];
                Vector3 value = Vector3.Cross(line,line2 );
                if (value.z > 0)//注意，根据左手法则，顺时针捏过去，如果大于0说明在左边，这里和xz平面不一样
                {
                    return false;
                }
            }
            Debug.Log($"点{point}在颜色{s.shape}内");
            return true;
        }


        /// <summary>
        /// 将顶点顺时针排序
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static List<Vector3> SortRound(Vector3[] vertices)
        {
            Vector3 min = vertices[0];
            int index = 0;
            for (int i = 1; i < vertices.Length; i++)
            {
                if (vertices[i].x < min.x)
                {
                    min = vertices[i];
                    index = i;
                }else if ( Mathf.Abs(vertices[i].x - min.x) < 1e-10 && vertices[i].y < min.y )
                {
                    min = vertices[i];
                    index = i;
                }
            }

            List<Vector3> list = new List<Vector3>(vertices.Length);
            list.Add(min);
            for (int i = 0; i < list.Capacity; i++)
            {
                if (i == index)
                {
                    continue;
                }
                list.Add(vertices[i]);
            }
            list.Sort((a,b) =>
            {
                if (b == min)
                {
                    return 0;
                }
                var aCos = Vector3.Dot((a - min).normalized, Vector3.up);
                var bCos = Vector3.Dot((b - min).normalized, Vector3.up);
                if (aCos > bCos)
                {
                    return -1;
                }
                else return 1;
            });
            return list;
        }
        
        
        #region Clipper

        /// <summary>
        /// 求异
        /// </summary>
        /// <param name="me"></param>
        /// <param name="you"></param>
        /// <returns></returns>
        public static PathsD Difference(Shape me,Shape you)
        {
            
            PathsD subject = new PathsD();
            /*PathD subPath = new PathD();*/
            Mesh meMesh = me.Filter.mesh;
            Vector3[] meVertices = meMesh.vertices;
            int[] triangles = meMesh.triangles;
            for (int i = 0; i < triangles.Length/3; i++)
            {
                int index = i * 3;
                PathD triangle = new PathD();
                for (int j = index; j < index+3; j++)
                {
                    Vector3 worldPos = me.Trans.TransformPoint(meVertices[triangles[j]]);
                    PointD pointD = new PointD(worldPos.x,worldPos.y) ;
                    triangle.Add(pointD);
                }
                subject.Add(triangle);
            }
            
            
            /*for (int i = 0; i < meVertices.Length ; i++)
            {
                Vector3 worldPos = me.Trans.TransformPoint(meVertices[i]);
                PointD pointD = new PointD(worldPos.x,worldPos.y) ;
                subPath.Add(pointD);
            }
            subject.Add(subPath);*/
            
            PathsD clip = new PathsD();
            /*PathD clipPath = new PathD();*/
            /*Vector3[] youVertices = me.Filter.mesh.vertices;
            for (int i = 0; i < meVertices.Length ; i++)
            {
                Vector3 worldPos = you.Trans.TransformPoint(youVertices[i]);
                PointD pointD = new PointD(worldPos.x,worldPos.y) ;
                clipPath.Add(pointD);
            }
            clip.Add(clipPath);*/
            Mesh youMesh = you.Filter.mesh;
            Vector3[] youVertices = youMesh.vertices;
            int[] triangles2 = youMesh.triangles;
            for (int i = 0; i < triangles2.Length/3; i++)
            {
                int index = i * 3;
                PathD triangle = new PathD();
                for (int j = index; j < index+3; j++)
                {
                    Vector3 worldPos = you.Trans.TransformPoint(youVertices[triangles2[j]]);
                    PointD pointD = new PointD(worldPos.x,worldPos.y) ;
                    triangle.Add(pointD);
                }
                clip.Add(triangle);
            }
            
            PathsD paths = Clipper.Difference(subject, clip, FillRule.EvenOdd);
            
            Debug.Log(paths.ToString());
            return paths;
        }

        public Mesh PolygonToMesh(PathsD pathsD,Shape s)
        {
            Transform trans = s.Trans;
            
            List<PolygonPoint> points = new List<PolygonPoint>();
            for (int i = 0; i < pathsD.Count; i++)
            {
                PathD pathD = pathsD[i];
                for (int j = pathD.Count - 1 ; j >= 0; j--)
                {
                    Vector3 localPoint = trans.InverseTransformPoint(new Vector3((float)pathD[j].x, (float)pathD[j].y, 0));
                    PolygonPoint point = new PolygonPoint(localPoint.x, localPoint.y);
                    /*vertices.Add(localPoint);*/
                    points.Add(point);
                }
            }

            Polygon polygon = new Polygon(points.ToArray());
            P2T.Triangulate(polygon);
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            var triangles = polygon.Triangles;
            for (int i = 0; i < triangles.Count; i++)
            {
                DelaunayTriangle triangle = triangles[i];
                for (int j = 0; j < 3; j++)
                {
                    TriangulationPoint p = triangle.Points[j];
                    Vector3 point = new Vector3((float)p.X, (float)p.Y, 0);
                    vertices.Add(point);
                    //由于库是求出的逆时针的三角形，所以需要倒着输入
                    indices.Add(i*3+2-triangle.IndexOf(p));
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();
            return mesh;
        }

        public Mesh PathDToMesh(PathsD pathsD,Shape s)
        {
            Transform trans = s.Trans;
            List<int> triangles = new List<int>();
            for (int i = 0; i < pathsD.Count; i++)
            {
                PathD pathD = pathsD[i];
                for (int j = 0 ; j < pathD.Count ; j+=3)
                {
                    int[] triangle;
                    int remainPoint = pathD.Count - j;
                    if (remainPoint >= 4)
                    {
                        triangle = new int[] { j,j+1,j+2, j,j+2,j+3};
                    }else if (remainPoint == 3)
                    {
                        triangle = new int[] { j,j+1,j+2,  j,j+2,0};
                    }else if (remainPoint == 2)
                    {
                        triangle = new int[] { j,j+1,0};
                    }
                    else
                    {
                        triangle = Array.Empty<int>();
                    }

                    if (triangle.Length > 0)
                    {
                        for (int k = 0; k < triangle.Length; k++)
                        {
                            triangles.Add(triangle[k]);
                        }
                    }

                    PolyPathD p;
                    
                }
            }

            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < pathsD.Count; i++)
            {
                PathD pathD = pathsD[i];
                for (int j = pathD.Count - 1 ; j >= 0; j--)
                {
                    Vector3 localPoint = trans.InverseTransformPoint(new Vector3((float)pathD[j].x, (float)pathD[j].y, 0));
                    vertices.Add(localPoint);
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            return mesh;
        }
        #endregion
    }
}
