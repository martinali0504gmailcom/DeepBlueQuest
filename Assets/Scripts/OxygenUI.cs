using UnityEngine;
using TMPro;

public class OxygenUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the LevelManager handling the oxygen logic.")]
    public LevelManager levelManager;
    [Tooltip("Reference to the TextMeshPro UI element displaying oxygen.")]
    public TextMeshProUGUI oxygenText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check to make sure the elements exist to prevent errors
        if (levelManager == null || oxygenText == null)
            return;

        //get time from LevelManager
        float timeRemaining = levelManager.oxygenTime;

        //convert it to minutes:seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        //Set the format and display the text (MM:SS)
        oxygenText.text = string.Format("Oxygen Remaining: {0:00}:{1:00}", minutes, seconds);
    }
}
