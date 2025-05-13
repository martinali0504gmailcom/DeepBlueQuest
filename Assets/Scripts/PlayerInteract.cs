using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteract : MonoBehaviour
{
    [Tooltip("Range where the player can interact with objects in the game world.")]
    public float interactRange = 3f;
    private PlayerControls controls;
    private PlayerInventory inventory;

    void Awake()
    {
        controls = new PlayerControls();
        inventory = GetComponent<PlayerInventory>();

        controls.Player.Interact.performed += ctx => OnInteract();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void OnInteract()
    {
        //Prevent interaction if the SpeechBubbleUI is showing
        if (SpeechBubbleUI.Instance != null && SpeechBubbleUI.Instance.IsShowing)
        {
            Debug.Log("SpeechBubbleUI is showing, cannot interact.");
            return;
        }

        Debug.Log("Attempting to interact.");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            //if it has an InteractableBase script, call .Interact()
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                Debug.Log("Attempting to interact with object.");
                interactable.Interact(inventory);
            }
            else
            {
                Debug.Log("Hit object that can't be interacted with.");
            }
        }
    }
}
