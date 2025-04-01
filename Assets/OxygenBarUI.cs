using UnityEngine;
using UnityEngine.UI;

public class OxygenBarUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("LevelManager that tracks the oxygenTime countdown.")]
    public LevelManager levelManager;

    [Tooltip("Slider that visually shows oxygen from 1 to 0.")]
    public Slider oxygenSlider;

    // We'll pull maxOxygen from levelManager.oxygenTime in Start()
    private float maxOxygen;

    void Start()
    {
        if (levelManager == null || oxygenSlider == null)
            return;

        // Copy the initial oxygenTime from the LevelManager
        maxOxygen = levelManager.oxygenTime;

        // Initialize the slider range
        oxygenSlider.minValue = 0f;
        oxygenSlider.maxValue = 1f;
        oxygenSlider.value = 1f;
        oxygenSlider.interactable = false;  // If you want to prevent user dragging
    }

    void Update()
    {
        if (levelManager == null || oxygenSlider == null)
            return;

        // Convert the current oxygenTime to a 0..1 ratio
        float ratio = levelManager.oxygenTime / maxOxygen;
        ratio = Mathf.Clamp01(ratio);

        // Update the slider
        oxygenSlider.value = ratio;
    }
}
