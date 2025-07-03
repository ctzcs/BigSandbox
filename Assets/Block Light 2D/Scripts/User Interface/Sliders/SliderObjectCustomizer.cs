using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Describes a slider object in the UI.
/// </summary>
public class SliderObjectCustomizer : SliderObject, IPointerEnterHandler
{
    /// <summary>
    /// Updates this slider and its related data object with a new slider value.
    /// Called automatically by the related slider's OnValueChanged event.
    /// If this slider activated the Perlin preview map, it updates the map as well.
    /// </summary>
    public override void EditField()
    {
        base.EditField();
        UserInterfaceTerrainify.Instance.UpdatePerlinPreviewMap();
    }


    /// <summary>
    /// Shows the Perlin preview map on mouse over if updating it was a success.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        UserInterfaceTerrainify.Instance.MousedOverSlider = this;
        if (!UserInterfaceTerrainify.Instance.perlinPreviewAllToggle.isOn)
            UserInterfaceTerrainify.Instance.UpdatePerlinPreviewMap();
    }
}
 