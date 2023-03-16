using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyFlowField
{
    public class UnitController : MonoBehaviour
    {
        public GridController gridController;
        public GameObject unitPrefab;
        public Transform spawnerPoint;
        public int numUnitsPerSpawn;
        public float moveSpeed;
     
        private List<GameObject> unitsInGame;
    
    	private void Awake()
    	{
    		unitsInGame = new List<GameObject>();
        }
    
    	void Update()
    	{
    		if (Input.GetKeyDown(KeyCode.Alpha1))
    		{
    			SpawnUnits();
    		}
    
    		if (Input.GetKeyDown(KeyCode.Alpha2))
    		{
    			DestroyUnits();
    		}
    	}
    
    	private void FixedUpdate()
    	{
    		if (gridController.curFlowField == null) { return; }
    		foreach (GameObject unit in unitsInGame)
    		{
    			var position = unit.transform.position;
                Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(position);
    			Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y,0);
    			// position += moveDirection * (Time.fixedDeltaTime * moveSpeed);
    			// unit.transform.position = position;
    			Rigidbody2D unitRB = unit.GetComponent<Rigidbody2D>();
    			unitRB.velocity = moveDirection * moveSpeed;
    		}
            UnitRender();
    	}
    
    	private void SpawnUnits()
    	{
    		Vector2Int gridSize = gridController.gridSize;
    		float nodeRadius = gridController.cellRadius;
    		Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius) + (Vector2)spawnerPoint.position;
    		int colMask = LayerMask.GetMask("Impassible", "Units");
    		Vector3 newPos;
    		for (int i = 0; i < numUnitsPerSpawn; i++)
    		{
    			GameObject newUnit = Instantiate(unitPrefab);
    			newUnit.transform.parent = transform;
    			unitsInGame.Add(newUnit);
    			// do
    			// {
    				newPos = new Vector3(Random.Range(0, maxSpawnPos.x), Random.Range(0, maxSpawnPos.y), 0);
    				newUnit.transform.position = newPos;
    			// }
    			// while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);
    		}
    	}
    
    	private void DestroyUnits()
    	{
    		foreach (GameObject go in unitsInGame)
    		{
    			Destroy(go);
    		}
    		unitsInGame.Clear();
    	}

        
        
        private void UnitRender()
        {
	        unitsInGame.Sort((a,b) =>
	        {
		        if (a.transform.position.y > b.transform.position.y)
		        {
			        return -1;
		        }
		        else
		        {
			        return 1;
		        }
	        });
	        // SortByDistance();
	        for (int i = 0; i < unitsInGame.Count; i++)
	        {
		        //这里也是可以优化的
		        SortingGroup c = unitsInGame[i].GetComponent<SortingGroup>();
		        if (c != null)
		        {
			        c.sortingOrder = i;
		        }
		        
	        }
        }

        private void SortByDistance()
        {
	        unitsInGame.Sort((a,b) =>
	        {
		        if (Vector3.Distance(a.transform.position,Vector3.zero) > Vector3.Distance(b.transform.position,Vector3.zero))
		        {
			        return -1;
		        }
		        else
		        {
			        return 1;
		        }
	        });
	        
	        
        }
        
    }

}