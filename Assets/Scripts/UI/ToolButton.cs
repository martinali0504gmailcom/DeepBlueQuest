using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    [Header("Tool Assignment")]
    [Tooltip("The toolTypes are listed in, and can be found in 'ToolType.cs'")]
    public ToolType toolType;
    public PlayerInventory inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get the button on this same GameObject
        Button btn = GetComponent<Button>();
        if (btn)
        {
            btn.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked()
    {
        //Equipped the specified tool
        inventory.EquipTool(toolType);
        Debug.Log("Equipped: " + toolType);
    }
}
