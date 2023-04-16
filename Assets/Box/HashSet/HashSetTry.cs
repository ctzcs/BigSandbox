using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashSetTry : MonoBehaviour
{
    public GameObject cube;
    private HashSet<Transform> _transHash;
    // Start is called before the first frame update
    void Start()
    {
        _transHash = new HashSet<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var obj = Instantiate(cube);
           obj.transform.position = 3*Random.onUnitSphere;
           _transHash.Add(obj.transform);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            pos.z = 10;
            var ray = Camera.main.ScreenPointToRay(pos);
            UnityEngine.Physics.Raycast(ray, out RaycastHit hitInfo);
            if (hitInfo.transform != null)
            {
                if (_transHash.Contains(hitInfo.transform))
                {
                    Debug.Log("包含");
                }
                else
                {
                    Debug.Log("不包含");
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var pos = Input.mousePosition;
            pos.z = 10;
            var ray = Camera.main.ScreenPointToRay(pos);
            UnityEngine.Physics.Raycast(ray, out RaycastHit hitInfo);
            if (hitInfo.transform != null)
            {
                if (_transHash.Contains(hitInfo.transform))
                {
                    _transHash.Remove(hitInfo.transform);
                }
                
            }
        }

        for (int i = 0; i < _transHash.Count; i++)
        {
            
        }
    }
}
