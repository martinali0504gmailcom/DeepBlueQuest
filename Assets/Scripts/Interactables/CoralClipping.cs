public class CoralClipping : InteractableBase
{
    public bool hasBeenSnipped = false;
    
    public override void Interact(PlayerInventory inventory)
    {
        base.Interact(inventory);

        if (inventory.HasToolEquipped(ToolType.CoralClippers) && !hasBeenSnipped)
        {
            hasBeenSnipped = true;
        }
    }
}