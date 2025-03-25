using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Above Water Settings")]
    [Tooltip("Gravity force when above water. E.g. 9.81 to replicate Earth's gravity.")]
    public float airGravity = 9.81f;

    [Header("Movement Settings")]
    [Tooltip("Forward/Back/Strafe speed (acceleration) in units per second.")]
    public float horizontalSpeed = 5f;

    [Tooltip("Ascend/Descend speed in units per second.")]
    public float verticalSpeed = 3f;

    [Header("Underwater Speed Limit")]
    [Tooltip("Maximum horizontal swim speed in m/s while underwater.")]
    public float maxUnderwaterSpeed = 8f;

    [Header("Buoyancy Settings")]
    [Tooltip("Y-position of the water's surface.")]
    public float surfaceLevel = 0f;

    [Tooltip("Depth where player buoyancy becomes 0.")]
    public float neutralBuoyancyDepth = -30f;

    [Tooltip("Upward force exerted near surface.")]
    public float buoyancyForce = 1f;

    [Tooltip("Base downward pull when deeper than neutral depth.")]
    public float gravityForce = 1f;

    [Header("Depth Intervals")]
    [Tooltip("Size of each 'depth interval' beyond neutral depth.")]
    public float depthIntervalSize = 5f;

    [Tooltip("Extra force added per interval beyond neutral depth.")]
    public float downwardForcePerInterval = 0.25f;

    [Header("Drag Settings")]
    [Tooltip("Rate at which velocity is lost per second underwater.")]
    public float waterDrag = 0.05f;

    [Header("Camera Settings")]
    [Tooltip("Horizontal rotation speed (yaw) & vertical (pitch) speed.")]
    public float rotationSpeed = 100f;

    [Tooltip("Minimum camera pitch (look down limit). E.g., -60.")]
    public float minPitch = -60f;

    [Tooltip("Maximum camera pitch (look up limit). E.g., 60.")]
    public float maxPitch = 60f;

    private PlayerControls controls;
    private CharacterController controller;

    private Transform camTransform;    // We'll rotate only for pitch
    private float cameraPitch;         // Tracks current pitch angle

    private Vector2 moveInput;         // (x = strafe, y = forward/back)
    private Vector2 lookInput;         // (x = yaw, y = pitch)
    private float verticalInput;       // +1 ascend, -1 descend

    // Single velocity vector storing both horizontal and vertical momentum.
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        controls = new PlayerControls();

        // Movement (WASD / stick)
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Looking (mouse or stick)
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        // Ascend
        controls.Player.Ascend.performed += ctx => verticalInput = 1f;
        controls.Player.Ascend.canceled += ctx => verticalInput = 0f;

        // Descend
        controls.Player.Descend.performed += ctx => verticalInput = -1f;
        controls.Player.Descend.canceled += ctx => verticalInput = 0f;
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;
        // Optionally lock the cursor for an FPS feel:
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    /// <summary>
    /// Rotates the player for yaw and the camera for pitch,
    /// with pitch clamped between minPitch and maxPitch.
    /// </summary>
    void HandleRotation()
    {
        // Yaw: rotate entire player so forward/back movement aligns with camera yaw
        float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, yaw, 0f);

        // Pitch: only rotate the camera transform, clamped
        cameraPitch -= lookInput.y * rotationSpeed * Time.deltaTime;  // subtract because up is negative delta
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

        // Apply pitch rotation to camera
        if (camTransform != null)
        {
            camTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    /// <summary>
    /// Incorporates horizontal momentum underwater, immediate stop above water,
    /// but clamps underwater horizontal speed to maxUnderwaterSpeed.
    /// </summary>
    void HandleMovement()
    {
        float depth = transform.position.y - surfaceLevel;

        // Build local movement direction from input
        Vector3 inputDir = (transform.forward * moveInput.y + transform.right * moveInput.x);

        if (depth <= 0f) // UNDERWATER
        {
            // 1) Accumulate horizontal velocity
            velocity += inputDir * horizontalSpeed * Time.deltaTime;

            // 2) Ascend/descend
            velocity.y += verticalInput * verticalSpeed * Time.deltaTime;

            // 3) Buoyancy
            velocity.y += CalculateBuoyancy() * Time.deltaTime;

            // 4) Water drag
            velocity *= (1f - waterDrag * Time.deltaTime);

            // 5) Clamp horizontal speed to maxUnderwaterSpeed
            ClampUnderwaterHorizontalSpeed();
        }
        else // ABOVE WATER
        {
            // 1) No horizontal momentum if no input. Overwrite velocity.xz.
            Vector3 horizontalNow = inputDir * horizontalSpeed;
            velocity.x = horizontalNow.x;
            velocity.z = horizontalNow.z;

            // 2) Ascend/descend
            velocity.y += verticalInput * verticalSpeed * Time.deltaTime;

            // 3) Air gravity if not grounded
            if (!controller.isGrounded)
            {
                velocity.y -= airGravity * Time.deltaTime;
            }
            else
            {
                if (velocity.y < 0f) velocity.y = 0f;
            }
        }

        // Move with CharacterController
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Clamps the horizontal part (x,z) of velocity so it never exceeds maxUnderwaterSpeed.
    /// </summary>
    void ClampUnderwaterHorizontalSpeed()
    {
        Vector2 horizontal = new Vector2(velocity.x, velocity.z);
        float speed = horizontal.magnitude;
        if (speed > maxUnderwaterSpeed)
        {
            // Limit to max speed
            horizontal = horizontal.normalized * maxUnderwaterSpeed;
            velocity.x = horizontal.x;
            velocity.z = horizontal.y;
        }
    }

    /// <summary>
    /// Applies buoyancy or extra downward force based on depth.
    /// If above surface => 0
    /// If between surface & neutral => linear fade from full buoyancy to 0
    /// If deeper than neutral => intervals add downward force
    /// returns positive => upward, negative => downward
    /// </summary>
    float CalculateBuoyancy()
    {
        float depth = transform.position.y - surfaceLevel;

        // Above surface => no buoyancy
        if (depth > 0f)
            return 0f;

        // Deeper than neutral => add intervals of extra downward force
        if (depth < neutralBuoyancyDepth)
        {
            float extraDepth = neutralBuoyancyDepth - depth; // negative minus negative => positive
            int intervals = Mathf.FloorToInt(extraDepth / depthIntervalSize);
            float totalDownForce = gravityForce + (intervals * downwardForcePerInterval);
            return -totalDownForce;
        }

        // Between surface and neutral => linear fade from full buoyancy to 0
        float ratio = depth / neutralBuoyancyDepth; // negative / negative => [0..1]
        return Mathf.Lerp(buoyancyForce, 0f, ratio);
    }
}