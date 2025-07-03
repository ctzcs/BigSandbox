using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Describes a simple item in the Customize grid section in the Terrainify 2D UI.
/// </summary>
[RequireComponent(typeof(Button), typeof(Image))]
public class GridItem : MonoBehaviour
{
    /* Every grid item points to a ScriptableObject of SliderDataCustomizer
     * to store its slider data. */
    public SliderDataCustomizer Data { get; set; }

    private Button button;
    private Image image;


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { SelectItem(); });

        image = GetComponent<Image>();
    }


    /// <summary>
    /// Assign a ScriptableObject to this GridItem to store its settings in.
    /// </summary>
    /// <param name="data"></param>
    public void InitializeItem(SliderDataCustomizer data)
    {
        Data = data;
        image.sprite = data.gridSprite;

        // Disable if this is the customizer item of the default terrain block.
        if (data == GenerationManager.Instance.defaultBlock)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears the selection of this item.
    /// </summary>
    public void DeselectItem()
    {
        image.color = Color.white;
    }

    /// <summary>
    /// Selects this item.
    /// </summary>
    public void SelectItem()
    {
        if (UserInterfaceTerrainify.Instance.SelectedItem != null)
            UserInterfaceTerrainify.Instance.SelectedItem.DeselectItem();
        UserInterfaceTerrainify.Instance.SelectedItem = this;
        image.color = Color.green;

        // Update all customizer UI sliders in the scene with the data of this GridItem
        UserInterfaceTerrainify.Instance.customizerTitle.text = Data.itemName;
        foreach (SliderObject customizerEntry in UserInterfaceTerrainify.Instance.CustomizerSliders)
        {
            customizerEntry.dataObject = Data;
            customizerEntry.InitializeField();
        }
    }
}