using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    [Header("References")]
    public NotificationUI notificationSystem;

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
            if (notificationSystem != null)
            {
                notificationSystem.ShowMessage("You do not have the proper tool equipped.");
            }
            else
            {
                Debug.LogWarning("Notification system is not assigned!");
            }
            return;
        }
        if (notificationSystem != null)
        {
            notificationSystem.ShowMessage("Interacted with " + gameObject.name + " using " + requiredTool + ".");
        }
        else
        {
            Debug.LogWarning("Notification system is not assigned!");
        }
        Debug.Log("Ineracted with " + gameObject.name + " using " + requiredTool);
    }
}
