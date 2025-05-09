using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Data References")]
    public LevelData levelData;
    public LevelsConfig levelsConfig;

    [Header("References")]
    public Transform player;
    public Transform start;
    public Transform goal;
    public PlayerMovementScript playerMovementScript;

    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    [Header("Oxygen Settings")]
    public float oxygenTime = 2700f;

    [Header("Game Settings")]
    public bool levelFailed = false;
    public bool levelComplete = false;
    public float goalRadius = 2f;
    public float menuReturnDelay = 4f;

    PlayerControls controls;
    Coroutine popupRoutine;          // keep handle so we can stop it
    Coroutine introRoutine;

    void Start()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Player.Enable();
        }

        if (player && start)
        {
            player.position = start.position;
            player.rotation = start.rotation;
        }

        HidePopup();

        string[] intro = (levelData.initialMessages != null && levelData.initialMessages.Length > 0)
            ? levelData.initialMessages : new string[] {levelData.initialText};

        introRoutine = StartCoroutine(ShowStringOfMessages(levelData.initialTextDismissTime, intro));

        SaveManager.LoadGame();
    }

    void Update()
    {
        // ------- close popup early on Interact ----------
        if (popupPanel.activeSelf && controls.Player.Interact.WasPressedThisFrame())
            HidePopup();
        // -------------------------------------------------

        if (levelFailed || levelComplete) return;

        if (player && player.position.y < playerMovementScript.surfaceLevel)
            oxygenTime -= Time.deltaTime;

        if (!levelFailed && oxygenTime <= 0f)
        {
            oxygenTime = 0f;
            OnLevelFail();
        }

        if (player && goal &&
            Vector3.Distance(player.position, goal.position) <= goalRadius)
        {
            OnLevelComplete();
        }
    }

    public void OnLevelFail()
    {
        levelFailed = true;
        ShowPopup(levelData.failText, levelData.failTextDismissTime);
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    public void OnLevelComplete()
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
        ShowPopup(levelData.successText, levelData.successTextDismissTime);
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(menuReturnDelay);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        SceneManager.LoadScene(0);
    }

    bool IsTutorialLevel(int idx)
    {
        if (levelsConfig == null) return false;
        foreach (int t in levelsConfig.tutorialLevels)
            if (t == idx) return true;
        return false;
    }

    // ---------------- popup helpers ----------------
    IEnumerator HidePopupAfterSeconds(float dur)
    {
        yield return new WaitForSeconds(dur);
        HidePopup();
    }

    //Show a series of messages instead of just one - TO FIX: Player hits the key and the whole text disappears
    IEnumerator ShowStringOfMessages(float dur, string[] msgs)
    {
        if(msgs == null || msgs.Length == 0) yield break;

        popupPanel.SetActive(true);

        foreach (string line in msgs)
        {
            popupText.text = line;

            float t = 0f;
            bool next = false;

            //Make sure the text isn't skipped multiple times
            yield return new WaitUntil(() => !controls.Player.Interact.IsPressed());

            while (!next)
            {
                t += Time.deltaTime;
                if (t >= dur)
                    next = true;
                if (controls.Player.Interact.WasPressedThisFrame())
                    next = true;      //Use interact to skip to next line

                yield return null;
            }
        }
        popupPanel.SetActive(false);
        introRoutine = null;
    }

    void ShowPopup(string msg, float dur = 3f)
    {
        if (popupPanel) popupPanel.SetActive(true);
        if (popupText)  popupText.text = msg;

        if (popupRoutine != null) StopCoroutine(popupRoutine);
        popupRoutine = StartCoroutine(HidePopupAfterSeconds(dur));
    }

    void HidePopup()
    {
        if (popupRoutine != null) StopCoroutine(popupRoutine);
        popupRoutine = null;
        if (popupPanel) popupPanel.SetActive(false);
    }
}
