using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBounds : MonoBehaviour
{
    private Collider2D _collider2D;
    // Start is called before the first frame update
    void Start()
    {
        _collider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            print(pos);
            print(_collider2D.bounds);
            //GC太多了
            if (_collider2D.bounds.Contains(pos))
            {
                OnMyTrigger();
            }
            else
            {
                print("不在");
            }
            


    }

    void OnMyTrigger()
    {
        
    }
}
