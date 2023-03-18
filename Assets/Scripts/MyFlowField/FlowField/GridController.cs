using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        //现在的格子
        private Cell nowCell;
        
        

        private void Start()
        {
            InitializeFlowField();
            curFlowField.CreateCostField();
            OverrideFlowField();
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
            // Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            // Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            //从鼠标转换到player坐标来绘制
            Cell destinationCell = curFlowField.GetCellFromWorldPos(player.transform.position);
            if (nowCell == destinationCell)
            {
                return;
            }
            nowCell = destinationCell;
            curFlowField.CreateIntegrationField(destinationCell);
            curFlowField.CreateFlowField();
            gridDebug.DrawFlowField();
        }
        
        private void FixedUpdate()
        {
            if (player.Vspeed != 0||player.Hspeed != 0)
            {
                OverrideFlowField();
            }

            
        }
    }

}