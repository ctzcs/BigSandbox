using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOne : MonoBehaviour
{
    private PolygonCollider2D _col;
    private Vector2[] _recordPoints;

    private LinkedList<Vector2> _pointsNode;
    // Start is called before the first frame update
    void Start()
    {
        _col = this.GetComponent<PolygonCollider2D>();
        _recordPoints = _col.points;
        _pointsNode = new LinkedList<Vector2>();
        RefreshPoints(_recordPoints,this.transform,ref _pointsNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        { 
            _col.enabled = true;
        }    
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        
    }

    public static void RefreshPoints(Vector2[] points,Transform center,ref LinkedList<Vector2> listNode)
    {
        listNode.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            listNode ??= new LinkedList<Vector2>();
            //将所有点转换到世界坐标系
            Vector2 point = center.TransformPoint(points[i]);
            listNode.AddLast(point);
#if UNITY_EDITOR
            Debug.Log(point);
#endif
        }

        
    }

    public static void TravelLines(LinkedList<Vector2> linkedList1,LinkedList<Vector2> linkedList2)
    {
        for (int i = 0; i < linkedList1.Count; i++)
        {
            
        }
    }
}
