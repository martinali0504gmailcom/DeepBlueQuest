// WaterFogController.cs
// Attach to the player (or an empty that follows the player).

using UnityEngine;

public class WaterFogController : MonoBehaviour
{
    [Header("General Settings")]
    public float surfaceY = 0f;            // filled from PlayerMovementScript
    public Camera gameCam;                 // auto-filled if left null

    [Header("Under-water Fog Colour")]
    public Color waterFogColor = new(0.11f, 0.35f, 0.55f, 1f);

    [Header("Density Settings")]
    public float baseDensity         = 0.02f;   // at the surface
    public float extraDensityPerMeter = 0.0003f;

    [Header("Far-clip Plane")]
    public float farClip = 600f;           // keep high; fog hides distant geo

    [Header("Player References")]
    public PlayerMovementScript playerMovementScript;   // to read surfaceLevel
    public Transform playerTransform;                   // diver position

    // ---- cached above-water values ----
    bool   startFogEnabled;
    Color  startFogColor;
    float  startFogDensity;
    FogMode startFogMode;
    float  startFarClip;
    CameraClearFlags startClearFlags;
    Color  startClearColor;

    void Start()
    {
        if (!gameCam) gameCam = Camera.main;
        if (!playerTransform) playerTransform = transform;

        // read the surface height from PlayerMovement if provided
        if (playerMovementScript != null)
            surfaceY = playerMovementScript.surfaceLevel;

        // ---- cache everything so we can restore it later ----
        startFogEnabled   = RenderSettings.fog;
        startFogColor     = RenderSettings.fogColor;
        startFogDensity   = RenderSettings.fogDensity;
        startFogMode      = RenderSettings.fogMode;

        startFarClip      = gameCam.farClipPlane;
        startClearFlags   = gameCam.clearFlags;
        startClearColor   = gameCam.backgroundColor;

        // set fog mode we intend to use under water (exp works on WebGL)
        RenderSettings.fogMode = FogMode.Exponential;
    }

    void Update()
    {
        bool underwater = playerTransform.position.y < surfaceY;

        if (underwater)
        {
            // -- enable & configure fog --
            RenderSettings.fog        = true;
            RenderSettings.fogColor   = waterFogColor;

            float depth   = Mathf.Abs(playerTransform.position.y - surfaceY);
            float density = baseDensity + depth * extraDensityPerMeter;
            RenderSettings.fogDensity = density;

            // make camera background same tint so no horizon seam
            gameCam.clearFlags      = CameraClearFlags.SolidColor;
            gameCam.backgroundColor = waterFogColor;

            gameCam.farClipPlane    = farClip;
        }
        else
        {
            // -- restore everything exactly how it was --
            RenderSettings.fog        = startFogEnabled;
            RenderSettings.fogColor   = startFogColor;
            RenderSettings.fogDensity = startFogDensity;
            RenderSettings.fogMode    = startFogMode;

            gameCam.clearFlags      = startClearFlags;
            gameCam.backgroundColor = startClearColor;
            gameCam.farClipPlane    = startFarClip;
        }
    }
}
