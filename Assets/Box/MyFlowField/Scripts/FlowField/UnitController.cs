using System.Collections.Generic;
using TreeEditor;
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
     
        private List<Transform> _unitsInGame;
        private List<Monster> _monsters;

    	private void Awake()
    	{
    		_unitsInGame = new List<Transform>(200);
            _monsters = new List<Monster>(200);

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

            if (_monsters.Count < 1) return; 
	            
	        for (int i = 0; i < _unitsInGame.Count; i++)
            {
	            var position = _unitsInGame[i].transform.position;
	            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(position);
	            Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y,0);
	            position += moveDirection * (Time.fixedDeltaTime * moveSpeed);
	            _monsters[i].Dir = moveDirection;
	            if (_monsters[i].CanMove)
	            {
		            _unitsInGame[i].position = position;
		            /*unit.GetComponent<Rigidbody2D>().velocity = moveSpeed * moveDirection;*/
		            /*unit.GetComponent<FBoid>().FlowPath(moveDirection);*/
	            }
	           
            }
    		
            
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
    			var newUnit = Instantiate(unitPrefab);
    			newUnit.transform.parent = transform;
    			_unitsInGame.Add(newUnit.transform);
    			// do
    			// {
    				newPos = new Vector3(Random.Range(0, maxSpawnPos.x), Random.Range(0, maxSpawnPos.y), 0);
    				newUnit.transform.position = newPos;
    			// }
    			// while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);
    		}

            for (int i = 0; i < _unitsInGame.Count; i++)
            {
	            _unitsInGame[i].TryGetComponent(out Monster m);
	            _monsters.Add(m);
            }
    	}

        private void DestroyUnits()
    	{
    		foreach (Transform go in _unitsInGame)
    		{
    			Destroy(go);
    		}
    		_unitsInGame.Clear();
    	}

    }
	
}