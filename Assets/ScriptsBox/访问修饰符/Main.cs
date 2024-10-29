using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private RootNode _root;
    // Start is called before the first frame update
    void Start()
    {
        _root = new RootNode();
        _root.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Other
{
    void Start(RootNode rootNode)
    {
        rootNode.Start();
    }
}

