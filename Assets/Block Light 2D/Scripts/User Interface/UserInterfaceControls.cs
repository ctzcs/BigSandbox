using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the UI elements for the controls window.
/// 
/// You can freely remove it from the game or disable it whenever you like, it won't
/// break anything. It's just there to help experiment with the features the asset has to offer.
/// </summary>
public class UserInterfaceControls : MonoBehaviour
{
    [Header("General")]
    public GameObject UIRoot;
    public RectTransform detailsWindow;
    public TextMeshProUGUI detailsButtonText;

    private Animator controlsAnimator;


    private void Awake()
    {
        controlsAnimator = detailsWindow.GetComponent<Animator>();
    }

    /// <summary>
    /// Shows or hides the controls window, depending on its current state.
    /// Called by the show/hide button of the controls window when clicked.
    /// </summary>
    public void ToggleControlsDisplay()
    {
        bool mode = !controlsAnimator.GetBool("ShowControls");
        controlsAnimator.SetBool("ShowControls", mode);
        detailsButtonText.text = mode ? "Hide Controls" : "Show Controls";
    }
}
