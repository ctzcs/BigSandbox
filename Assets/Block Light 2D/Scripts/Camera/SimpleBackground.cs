using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Very simple background control. Adjusts night sky visibility based on night level.
/// </summary>
public class SimpleBackground : MonoBehaviour
{
    private Camera mainCamera;
    private Canvas canvas;
    private RawImage nightBackground;

    
    private void Awake()
    {
        mainCamera = Camera.main;
        canvas = GetComponent<Canvas>();
        nightBackground = GetComponentInChildren<RawImage>();

        canvas.worldCamera = mainCamera;
    }

    private void Update()
    {
        if (nightBackground != null && mainCamera != null)
        {
            nightBackground.color = new Color(
                nightBackground.color.r,
                nightBackground.color.g,
                nightBackground.color.b, Mathf.Clamp(LightingManager.Instance.NightLevel - 0.5f, 0f, 1f));
        }
    }
}