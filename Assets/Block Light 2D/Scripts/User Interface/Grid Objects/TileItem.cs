using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple tile grid item in the lighting UI.
/// </summary>
[RequireComponent(typeof(Button), typeof(Image))]
public class TileItem : MonoBehaviour
{
    public Chunk.TileType tile;

    private Button button;
    private Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { SelectTile(); });

        image = GetComponent<Image>();
    }

    /// <summary>
    /// Clears the selection and ghosting of this tile.
    /// </summary>
    public void DeselectTile()
    {
        image.color = Color.white;

        if (ToolManager.Instance.IsGhosting)
            ToolManager.Instance.Unghost();
    }

    /// <summary>
    /// Selects this tile and enable ghosting.
    /// </summary>
    public void SelectTile()
    {
        if (UserInterfaceLighting.Instance.SelectedTile != null)
            UserInterfaceLighting.Instance.SelectedTile.DeselectTile();
        UserInterfaceLighting.Instance.SelectedTile = this;

        image.color = Color.green;
        ToolManager.Instance.GhostTile(GenerationManager.Instance.blockData[(int)tile] as SliderDataCustomizer);
    }
}
