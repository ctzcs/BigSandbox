using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Describes a slider object in the UI.
/// </summary>
public class SliderObject : MonoBehaviour
{
    public Slider inputSlider;        
    public SliderData.SliderField field;
    public SliderData dataObject;

    protected InputField inputField;
    protected readonly string decimalFormatting = "F3";


    private void Awake()
    {
        inputField = GetComponentInChildren<InputField>();
    }


    private IEnumerator Start()
    {
        yield return null;
        InitializeField();
    }


    /// <summary>
    /// Initializes by grabbing data from the assigned data object.
    /// </summary>
    public void InitializeField()
    {
        if (dataObject == null)
            return;

        inputSlider.value = dataObject.GetSliderData(field);
    }


    /// <summary>
    /// Updates this slider and its related data scriptable object with a new slider value.
    /// Called automatically by the related slider's OnValueChanged event.
    /// </summary>
    public virtual void EditField()
    {
        float inputValue = inputSlider.value;
        dataObject.SetSliderData(field, inputValue);
        inputField.text = !inputSlider.wholeNumbers ?
            inputValue.ToString(decimalFormatting) : inputValue.ToString();

        // Save changes to the data object if we're in the Editor
#if UNITY_EDITOR
        EditorUtility.SetDirty(dataObject);
#endif
    }

    /// <summary>
    /// Called when the input field value has changed. Simply adjusts the slider and calls EditField();
    /// </summary>
    public void OnInputFieldChanged()
    {
        inputSlider.value = float.Parse(inputField.text);
        EditField();
    }
}