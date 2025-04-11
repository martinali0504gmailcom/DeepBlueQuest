using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    [Header("Tool Required to Interact With")]
    public ToolType requiredTool = ToolType.None;

    [Tooltip("If true, we can hold the key.")]
    public bool requiresHold = false;
    public float timeToHold = 0f;

    public virtual void Interact(PlayerInventory inventory)
    {
        if(!inventory.HasToolEquipped(requiredTool))
        {
            Debug.Log("You do not have the proper tool equipped.");
            return;
        }
        Debug.Log("Ineracted with " + gameObject.name + " using " + requiredTool);
    }
}
