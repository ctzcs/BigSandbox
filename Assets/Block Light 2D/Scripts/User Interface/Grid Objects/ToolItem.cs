using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple tool grid item in the lighting UI.
/// </summary>
[RequireComponent(typeof(Button), typeof(Image))]
public class ToolItem : MonoBehaviour
{
    public ToolType tool;
    public enum ToolType
    {
        PLACE_TILE,
        REMOVE_TILE,
        PLACE_LIGHT,        
        REMOVE_LIGHT
    }

    private Button button;
    private Image image;


	private void Awake ()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { SelectTool(); });

        image = GetComponent<Image>();
    }

    /// <summary>
    /// Clears the selection and ghosting of this tool.
    /// </summary>
    public void DeselectTool()
    {
        image.color = Color.white;

        if (ToolManager.Instance.IsGhosting)
            ToolManager.Instance.Unghost();
    }

    /// <summary>
    /// Selects this tool and enable ghosting.
    /// </summary>
    public void SelectTool()
    {
        if (UserInterfaceLighting.Instance.SelectedTool != null)
            UserInterfaceLighting.Instance.SelectedTool.DeselectTool();
        UserInterfaceLighting.Instance.SelectedTool = this;

        image.color = Color.green;
        UserInterfaceLighting.Instance.tileSelectRoot.SetActive(false);        
        switch (tool)
        {
            case ToolType.PLACE_TILE:
                UserInterfaceLighting.Instance.tileSelectRoot.SetActive(true);
                if (UserInterfaceLighting.Instance.SelectedTile == null)
                    UserInterfaceLighting.Instance.SelectedTile = 
                        UserInterfaceLighting.Instance.tileSelectRoot.GetComponentInChildren<TileItem>();
                UserInterfaceLighting.Instance.SelectedTile.SelectTile();
                break;
            case ToolType.REMOVE_TILE:
                break;
            case ToolType.PLACE_LIGHT:
                ToolManager.Instance.GhostTile(ToolManager.Instance.lightSpriteAdd);
                break;
            case ToolType.REMOVE_LIGHT:
                ToolManager.Instance.GhostTile(ToolManager.Instance.lightSpriteRemove);
                break;
            default:
                break;
        }
    }
}
