using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Player Tools")]
    public ToolType currentTool = ToolType.None;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EquipTool(ToolType.None);
    }

    public void EquipTool(ToolType toolToEquip)
    {
        currentTool = toolToEquip;
        //debug message
        Debug.Log("Equpped tool: " + currentTool);
    }

    public bool HasToolEquipped (ToolType tool)
    {
        return currentTool == tool;
    }
}
