using UnityEngine;


//Set up for the rock glue objectives only
[RequireComponent(typeof(RockGlue))]
public class ObjectiveArrowIndicator : MonoBehaviour
{
    [Tooltip("Drag PlayerInventory (from the Player root)")]
    public PlayerInventory playerInventory;

    [Tooltip("Child GameObject to serve as the floating arrow")]
    public GameObject arrowIndicator;

    RockGlue rockGlue;

    void Awake()
    {
        rockGlue = GetComponent<RockGlue>();
        if (arrowIndicator != null)
            arrowIndicator.SetActive(false);
    }

    void Update()
    {
        if (playerInventory == null || arrowIndicator == null) return;

        // show arrow if player has at least one sample and rock still needs placement
        bool shouldShow = playerInventory.SampleCount > 0 && !rockGlue.HasCoralPlaced;
        arrowIndicator.SetActive(shouldShow);
    }
}