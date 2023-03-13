using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MyFlowField
{
    public class GridController : MonoBehaviour
    {
        public Vector2Int gridSize;
        public float cellRadius;
        public FlowField curFlowField;
        public GridDebug gridDebug;
        public Transform spawnerPoint;
        public Player player;
        private Cell nowCell;
        private void InitializeFlowField()
        {
            curFlowField = new FlowField(cellRadius, gridSize);
            curFlowField.CreateGrid(spawnerPoint.position);
            gridDebug.SetFlowField(curFlowField);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                
            }

            if (player.Vspeed != 0||player.Hspeed != 0)
            {
                InitializeFlowField();

                curFlowField.CreateCostField();

                // Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
                // Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
                //从鼠标转换到player坐标来绘制
                Cell destinationCell = curFlowField.GetCellFromWorldPos(player.transform.position);
                if (nowCell != destinationCell)
                {
                    nowCell = destinationCell;
                    
                    curFlowField.CreateIntegrationField(destinationCell);

                    curFlowField.CreateFlowField();

                    gridDebug.DrawFlowField();
                }
                
            }

            
        }
    }

}