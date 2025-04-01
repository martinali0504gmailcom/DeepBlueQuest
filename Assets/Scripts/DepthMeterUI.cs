using UnityEngine;
using UnityEngine.UI;       // for Text (legacy)
using TMPro;               // only if using TextMeshPro

public class DepthMeter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the player's movement script that stores surfaceLevel and the transform.")]
    public PlayerMovementScript playerMovement;

    [Tooltip("TextMeshPro or Text UI element that will display the depth.")]
    public TextMeshProUGUI depthTextTMP;  // If using TextMeshPro
    // OR if using legacy Text:
    // public Text depthTextUI;

    [Header("Display Settings")]
    [Tooltip("If true, we display integer-only depth (e.g. 5). If false, we can show decimals (e.g. 5.2).")]
    public bool showWholeNumbers = true;

    void Update()
    {
        if (playerMovement == null) return;

        // We assume surfaceLevel is the Y coordinate for the water surface.
        float surface = playerMovement.surfaceLevel;
        float currentY = playerMovement.transform.position.y;

        // Depth is how far below the surface we are. If above, we clamp to 0.
        float depthBelowSurface = surface - currentY;
        if (depthBelowSurface < 0f)
            depthBelowSurface = 0f;

        // Convert to a friendly string
        string depthString = showWholeNumbers
            ? Mathf.RoundToInt(depthBelowSurface).ToString()
            : depthBelowSurface.ToString("F1");

        // E.g. "Depth: 12m"
        string finalText = "Depth: " + depthString + "m";

        // Update the UI text
        if (depthTextTMP != null)
        {
            depthTextTMP.text = finalText;
        }
        // else if (depthTextUI != null)
        // {
        //    depthTextUI.text = finalText;
        // }
    }
}
