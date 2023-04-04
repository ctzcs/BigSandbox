
using System.Collections;
using UnityEngine;


namespace MyFlowField
{
    public class GridController : MonoBehaviour
    {
        //格子的横纵数量
        public Vector2Int gridSize;
        //格子的半径
        public float cellRadius;
        //当前的流场
        public FlowField curFlowField;
        //格子的debug
        public GridDebug gridDebug;
        //格子生成的起点
        public Transform spawnerPoint;
        //关注的player相当于关注的目标点
        public Player player;

        private Transform _player;
        //现在的格子
        private Cell _nowCell;
        private bool _r;

        private void Start()
        {
            _player = player.transform;
            //方式一：同一线程的方法
            /*InitializeFlowField();
            curFlowField.CreateCostField();
            OverrideFlowField();*/
            
            //方式二：多线程方法，通过携程隔一段时间调用一次
            PathRequest request = new PathRequest(_player.position,spawnerPoint.position,gridSize,cellRadius,GetField);
            FieldRequestManager.FlowFieldRequest(request);
            
            
            _r = true;
            /*StartCoroutine("ThreadRequest");*/
        }

        void GetField(FlowField field, bool b)
        {
            if (b)
            {
                curFlowField = field;
            }
        }
        /// <summary>
        /// 初始化场域
        /// </summary>
        private void InitializeFlowField()
        {
            curFlowField = new FlowField(cellRadius, gridSize,spawnerPoint.position);
            curFlowField.CreateGrid();
            gridDebug.SetFlowField(curFlowField);
        }
        
        
        
        /// <summary>
        /// 由于场域是规则的，所以可以每帧重写场域中的数据
        /// </summary>
        private void OverrideFlowField()
        {
            //从鼠标转换到player坐标来绘制
            Cell destinationCell = curFlowField.GetCellFromWorldPos(_player.position);
            if (_nowCell == destinationCell)
            {
                return;
            }
            _nowCell = destinationCell;
            curFlowField.CreateIntegrationField(destinationCell);
            curFlowField.CreateFlowField();
#if UNITY_EDITOR
            gridDebug.DrawFlowField();
#endif
            
        }
        

        private void FixedUpdate()
        {
            if (player.Vspeed != 0||player.Hspeed != 0)
            {
                /*countPer++;
                if (countPer >= 30)
                {
                    OverrideFlowField();
                    
                    countPer = 0;
                }*/
                PathRequest request = new PathRequest(_player.position,spawnerPoint.position,gridSize,cellRadius,GetField);
                FieldRequestManager.FlowFieldRequest(request);
                
            }
        }

        /// <summary>
        /// 发出线程请求的携程
        /// </summary>
        /// <returns></returns>
        IEnumerator ThreadRequest()
        {
            while (_r)
            {
                yield return new WaitForSeconds(0.5f);
                //方式一
                /*OverrideFlowField();*/
                
                //方式二
                PathRequest request = new PathRequest(_player.position,spawnerPoint.position,gridSize,cellRadius,GetField);
                FieldRequestManager.FlowFieldRequest(request);
                
                
                
            }
        }
    }

}