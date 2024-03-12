using System;
using Unity.Mathematics;
using UnityEngine;

namespace RenderBox.SandSimulate
{
    /// <summary>
    /// 存放沙的细胞
    /// </summary>
    public class Cell
    {
        public SandPixel pixel;
        public Vector2Int index;
    }
    /// <summary>
    /// 沙的类型
    /// </summary>
    public enum ESandType
    {
        Solid,
    }

    /// <summary>
    /// 沙的状态
    /// </summary>
    public enum ESandState
    {
        Falling,
        Static,
    }
    
    public class SandPixel
    {
        public ESandType sandType;
        public ESandState sandState;
        /// <summary>
        /// 当前颜色
        /// </summary>
        public Color nowColor;
        
    }
    public class SandSimulate : MonoBehaviour
    {
        public Color defaultColor = Color.white;
        public GameObject scene;
        public Renderer rd;
        public Texture2D texture2D;
        public Camera c;
        private static readonly int mainTex = Shader.PropertyToID("_BaseMap");
        private int size;
        public Cell[,] cellArray;

        private Color[] colors;

        
        // Start is called before the first frame update
        void Awake()
        {
            size = 64;
            cellArray = new Cell[size,size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    cellArray[i, j] = new Cell()
                    {
                        index = new Vector2Int(i,j),
                    };
                }
            }
            
            colors = new Color[size * size];
            
            c = Camera.main;
            texture2D = new Texture2D(size,size)
            {
                filterMode = FilterMode.Point
            };
            texture2D.SetPixel(10,10,Color.black);
            texture2D.Apply();
            rd.material.SetTexture(mainTex,texture2D);
            Application.targetFrameRate = 60;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var pos = Input.mousePosition;
                pos.z = 0;
                var worldPos = c.ScreenToWorldPoint(pos);
                var localPos = scene.transform.InverseTransformPoint(worldPos);
                if (localPos.x > 0.5 || localPos.y > 0.5 || localPos.x < -0.5 || localPos.y < -0.5)
                {
                    
                }
                else
                {
                    Paint(GetIndex(localPos)); 
                }
            }

            if (cellArray is not null)
            {
                Fall();
            }
            
        }



        ValueTuple<int,int> GetIndex(Vector2 pos)
        {
            var cellPos = pos + Vector2.one / 2;
            int x =  (int)math.ceil(cellPos.x * size) - 1;
            int y = (int)math.ceil(cellPos.y * size) - 1;
            return new ValueTuple<int,int>(x, y);
        }


        void Paint(ValueTuple<int, int> index)
        {
            int x = index.Item1;
            int y = index.Item2;
            cellArray[x, y].pixel ??= new SandPixel()
            {
                nowColor = Color.black,
                sandType = ESandType.Solid,
                sandState = ESandState.Falling
            };
        }
        
        

        void Fall()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size;j++)
                {
                    Step(i,j);
                }
            }
            //获取所有的颜色
            //TODO 不知道为什么这里的颜色需要镜像
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (cellArray[i,j].pixel is null)
                    {
                        colors[j * size + i] = defaultColor;

                    }
                    else
                    {
                        colors[j*size + i] = cellArray[i, j].pixel.nowColor;
                    }
                    
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply();
        }

        void Step(int x,int y)
        {
            Cell now = cellArray[x,y];
            if (now.pixel == null)
            {
                return;
            }
            if ( y > 0)
            {
                if (cellArray[x,y-1].pixel is null)
                {
                    cellArray[x,y-1].pixel = cellArray[x, y].pixel;
                    cellArray[x, y].pixel = null;
                }
                else
                {
                    cellArray[x, y].pixel.sandState = ESandState.Static;
                }
            }
            else if(y == 0)
            {
                now.pixel.sandState = ESandState.Static;
            }
        }

        
    }
}
