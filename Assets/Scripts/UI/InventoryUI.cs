using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public static InventoryUI Instance;
    public GameObject inventoryPanel;
    private PlayerControls controls;
    public PlayerMovementScript playerMovementScript;
    
    bool isPanelOpen = false;

    void Awake()
    {
        Instance = this; //Check for the instance of InventoryUI
        controls = new PlayerControls();

        //Bind the inventory button(s)
        controls.Player.Inventory.performed += ctx => ToggleInventory();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleInventory()
    {
        isPanelOpen = !isPanelOpen;
        inventoryPanel.SetActive(isPanelOpen);

        Cursor.lockState = isPanelOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPanelOpen;

        playerMovementScript.SetCameraLock(isPanelOpen); // Lock or unlock the camera based on inventory state
    }
}
