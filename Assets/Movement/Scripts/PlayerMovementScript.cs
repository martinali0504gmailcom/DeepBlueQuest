using UnityEngine;
using UnityEngine.InputSystem; //Gets the needed components to use Unity's "Input System"


public class PlayerMovementScript : MonoBehaviour
{
    [Header("Above Water Settings")]
    [Tooltip("Gravity force when above water. Should be about 9.81 to replicat Earth's gravity.")]
    public float airGravity = 9.81f;

    [Header("Movement Settings")]
    [Tooltip("Forward/Back/Strafe speed in untis per second (float)")]
    public float horizontalSpeed = 5f;             //Horizontal Move Speed
    [Tooltip("Ascend/Descend speed in untis per second (float)")]
    public float verticalSpeed = 3f;             //Up and down move speed
    [Header("Buoyancy Settings")]
    [Tooltip("Y-Position of the water's surface (float)")]
    public float surfaceLevel = 0f;
    [Tooltip("Depth where player buoyancy becoes 0 (float)")]
    public float neutralBuoyancyDepth = -30f;

    [Tooltip("Upward force exerted on player (float)")]
    public float buoyancyForce = 1f;              //Passive water upwards movement
    [Tooltip("Base downward pull when deeper than neutral buoyancy depth (float)")]
    public float gravityForce = 1f;

    [Header("Depth Intervals")]
    [Tooltip("Size of each 'depth interval' beyond neutral buoyancy depth")]
    public float depthIntervalSize = 5f;

    [Tooltip("How much extra force is added per interval beyond neutral depth (float)")]
    public float downwardForcePerInterval = 0.25f;

    [Header("Drag Settings")]
    [Tooltip("Rate where velocity is lost each second")]
    public float waterDrag = 0.05f;          //How fast you lose momentium
    [Header("Camera Settings")]
    [Tooltip("Camera rotation speed")]
    public float roationSpeed = 100f;        //Looking sensitivity speed
    

    private PlayerControls controls;
    private Vector3 velocity;

    private CharacterController controller;
    private Transform camTransform;         //Controls pitch

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalInput;
    private void Awake()
    {
        controls = new PlayerControls();

        //Create and wire input actions
        //'ctx' is the InputAction.CallbackContext providing input details     
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Ascend.performed += ctx => verticalInput = 1f;        
        controls.Player.Ascend.canceled += ctx => verticalInput = 0f;   

        controls.Player.Descend.performed += ctx => verticalInput = -1f;    
        controls.Player.Descend.canceled += ctx => verticalInput = 0f;  

    }
    private void OnEnable()
        //Instantiate (create) our new input actions
    {
        //Enable the "Player" action map so it can "listen" for inputs
        controls.Player.Enable();
   
    }

    private void OnDisable()
    {
        //Disable when the player object is NOT in use
        controls.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assume the camera is a child in the hierachy
        camTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();

        //Lock cursor for FPS control (disabled for now)
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        HandleMovement();
        HandleRotation();
    }


    /*
    Get the velocity based on input and buoyance, then applies drag.
    After, it moves the characterController (to prevent no-clipping)
    */
    void HandleMovement()
    {
        float depth = transform.position.y - surfaceLevel;
        //Step 1. Read the input
        Vector3 inputDir = (transform.forward * moveInput.y) + (transform.right * moveInput.x);

        //Step 2. Get horizontal momentum, keeping momentum if input stops
        velocity += inputDir * horizontalSpeed * Time.deltaTime;

        if (depth > 0f && Vector3.zero == inputDir)
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        //Step 3. Handle ascent/descent control
        //If we are above the surface and trying to fly, prevent it.
        velocity.y += verticalInput * verticalSpeed * Time.deltaTime;

        //Step 4. Apply buoyancy/downward force
        velocity.y += CalculateBuoyancy() * Time.deltaTime;

        //Step 5. Apply Drag (only if under water)
        if (depth < 0f)
        {
            velocity *= (1f - waterDrag * Time.deltaTime);
        }

        //Step 6. Move the character
        controller.Move(velocity * Time.deltaTime);
    }

    /*
    Applies a forced based on depth.
    If above surface => 0
    If between surface and neutral -> Linear face from full to 0
    If deeper than neutral => add downward force
    Positive values float up, negative makes you sink
    */
    float CalculateBuoyancy()
    {
        //If negative depth, assume underwater
        float depth = transform.position.y - surfaceLevel;

        //If Above surface, assume no buoyancy is requried
        if (depth > 0f)
            return 0f;

        if (depth < neutralBuoyancyDepth )
        {
            //if neutralBD is -15, and we are at y = - 30, thats 15 units deeper
            float extraDepth = neutralBuoyancyDepth - depth;
            int intervals = Mathf.FloorToInt(extraDepth / depthIntervalSize);
            float totalDownwardForce = gravityForce + (intervals * downwardForcePerInterval);
            return -totalDownwardForce;
        }
        if (depth >= surfaceLevel)
            return airGravity;

        float ratio = depth/neutralBuoyancyDepth;
        return Mathf.Lerp(buoyancyForce, 0f, ratio);

    }

    void HandleRotation()
    {
        camTransform.Rotate(0f, lookInput.x, 0f);

        if (camTransform != null)
        {
            camTransform.Rotate(-lookInput.y, 0f, 0f);
        }
    }
}
