using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; //Needed for scene control

public class LevelManager : MonoBehaviour
{
    [Header("Data References")]
    [Tooltip("Place the data (text for popup box) for the level here.")]
    public LevelData levelData;

    [Header("References")]
    public Transform player;
    [Tooltip("To change where the player starts to face, change the X-Valye of Start's rotation.")]
    public Transform start;
    public Transform goal;

    [Tooltip("UI Panel that pops up.")]
    public GameObject popupPanel;
    [Tooltip("TMP text that displays messages in the popup.")]
    public TextMeshProUGUI popupText;

    [Header("Oxygen Settings")]
    [Tooltip("Time till oxygen depletes: Recall that 60 seconds = 1 minute, so 2700 sec = 45 mins")]
    public float oxygenTime = 2700f; // Unity counts in seconds; 60f = 1 min -> 2700 seconds = 45 min

    [Header("Game Settings")]
    public bool levelFailed = false;
    public bool levelComplete = false;
    public float goalRadius = 2f; //Distance we consider the player has reached the goal

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Spawn player at "start" location
        if (player != null && start != null)
        {
            player.position = start.position;
            player.rotation = start.rotation; //To rotate, change the X-value of the start's rotation.
        }

        HidePopup();
        //Clarify the objective quickly
        ShowPopup(levelData.initialText, levelData.initialTextDismissTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (levelFailed || levelComplete)
            return; //Stop updating if the level is over:

        oxygenTime -= Time.deltaTime;
        if (!levelFailed && oxygenTime <= 0f)
        {
            oxygenTime = 0f;
            OnLevelFail();
        }

        if (player != null && goal != null)
        {
            float distanceToGoal = Vector3.Distance(player.position, goal.position);
            if (distanceToGoal <= goalRadius)
            {
                OnLevelComplete();
            }
        }
    }

    void OnLevelFail()
    {
        levelFailed = true;
        Debug.Log("Oxygen depleted! You have failed the level!");
        ShowPopup(levelData.failText, levelData.failTextDismissTime);
    }

    void OnLevelComplete()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (!IsTutorialLevel(buildIndex))
        {
            int highest = SaveManager.currentData.highestUnlockedLevel;
            if (buildIndex >= highest)
            {
                SaveManager.currentData.highestUnlockedLevel = buildIndex + 1;
                SaveManager.SaveGame();
            }
        }

        levelComplete = true;
        Debug.Log("Level Complete! You reached the goal!");
        ShowPopup(levelData.successText, levelData.successTextDismissTime);

        SceneManager.LoadScene(0);
    }

    bool IsTutorialLevel(int index)
    {
        var config = Resources.Load<LevelsConfig>("LevelsConfig");
        foreach (int t in config.tutorialLevels)
        {
            if (t == index) return true;
        }
        return false;
    }

    //Corotine to hide popup after a specified time
    private IEnumerator HidePopupAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration); //Wait the duration time before acting
        HidePopup();
    }

    //Method that shows the user a message on the popup!
    //Put in the string of the mesaage to be shown to the player
    //(Optional), then, put in the time the popup is to be displayed.
    void ShowPopup(string message, float popupTime = 3f)
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);

        if (popupText != null)
            popupText.text = message;
        else 
        Debug.Log("ERROR: Failed to display popup message!");//check for errors

        StopAllCoroutines(); //cancels previous hide timers
        //Hide the popup after the specified time (or 3 seconds, if none was given)
        StartCoroutine(HidePopupAfterSeconds(popupTime));
    }

    void HidePopup()
    {
        if (popupPanel) popupPanel.SetActive(false);
    }
}
