using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Very simple movement script for the camera.
/// </summary>
public class SimpleCameraBehaviour : MonoBehaviour
{
    public float speed = 32f;
    public float zoomIncrement = 4f;
    public float cameraZoomMin = 4f, cameraZoomMax = 256f;

    private float scrollwheelInput;

    private void Update ()
    {
        // Simple movement
        transform.position += new Vector3(
            Input.GetAxis("Horizontal") * Time.deltaTime * speed,
            Input.GetAxis("Vertical") * Time.deltaTime * speed);

        // Detect zoom input
        scrollwheelInput = Input.GetAxis("Mouse ScrollWheel");

        // Only zoom when not using UI key combinations
        if (scrollwheelInput != 0f && !Input.GetKey(KeyCode.LeftShift) && 
            !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt))
            Zoom(scrollwheelInput < 0f ? zoomIncrement : -zoomIncrement);
    }


    /// <summary>
    /// Zooms the camera with the given relative change to apply.
    /// Automatically clamps between the minimal and maximal zoom.
    /// </summary>
    /// <param name="relativeChange"></param>
    public void Zoom(float relativeChange)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + relativeChange, cameraZoomMin, cameraZoomMax);
    }
}