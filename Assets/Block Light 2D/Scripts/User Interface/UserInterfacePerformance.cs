using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI elements for the performance section.
/// 
/// You can freely remove it from the game or disable it whenever you like, it won't
/// break anything. It's just there to help experiment with the features the asset has to offer.
/// </summary>
public class UserInterfacePerformance : Singleton<UserInterfacePerformance>
{
    private Canvas canvas;


    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }
}
