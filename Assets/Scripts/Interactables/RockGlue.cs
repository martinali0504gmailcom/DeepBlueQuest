using UnityEngine;

public class RockGlue : InteractableBase
{
    [Tooltip("Where the baby coral should appear")]
    public Transform spawnPoint;
    [Tooltip("Prefab of the baby coral")]
    public GameObject babyCoralPrefab;

    public bool coralPlaced = false;

    public override void Interact(PlayerInventory inv)
    {
        if (coralPlaced) return;

        if (!inv.HasToolEquipped(ToolType.CoralGlue))
        {
            notificationSystem?.ShowMessage("Need coral glue!", 2f);
            return;
        }

        if (!inv.ConsumeSample())
        {
            notificationSystem?.ShowMessage("You have no coral samples!", 2f);
            return;
        }

        // spawn coral
        Instantiate(babyCoralPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        coralPlaced = true;
        notificationSystem?.ShowMessage("Coral glued!");

        LevelObjectiveTracker.Instance.NotifyCoralGlued(this);
    }
}
