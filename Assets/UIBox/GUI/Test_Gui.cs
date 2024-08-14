using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Gui : MonoBehaviour
{
    private void OnGUI()
    {
        //GUI.BeginGroup(new Rect(new Vector2(100,100),new Vector2(2,2)));
        GUI.Button(new Rect(new Vector2(100,100),new Vector2(100,100)),"a");//new Rect(new Vector2(100,100),new Vector2(2,2));
    }
}
