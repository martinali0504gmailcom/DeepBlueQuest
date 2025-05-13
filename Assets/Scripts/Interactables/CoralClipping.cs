using UnityEngine;

public class CoralClipping : InteractableBase
{
    [Header("Visuals")]
    [Tooltip("Mesh or GameObject that should disappear once snipped.")]
    public GameObject coralMesh;

    public bool hasBeenSnipped = false;
    
    public override void Interact(PlayerInventory inventory)
    {
        base.Interact(inventory);

        if (!inventory.HasToolEquipped(ToolType.CoralClippers))
        {
            SpeechBubbleUI.Instance.Show("You need Clippers!", 2f);
            return;
        }

        
        if (hasBeenSnipped)
        {
            SpeechBubbleUI.Instance.Show("This Coral is already snipped!", 2f);
            return;
        }

        hasBeenSnipped = true;

        // Hide the coral mesh
        if (coralMesh)
        {
            coralMesh.SetActive(false);
        }

        // Give the player a coral sample
        inventory.GiveSample();
        SpeechBubbleUI.Instance.Show("You got a piece of coral, now go find a spot for it!", 2f);
        // notificationSystem?.ShowMessage("Coral Snipped!", 2f);

        // tell LevelManager one coral finished
        LevelObjectiveTracker.Instance.NotifyCoralSnipped(this);
    }
}