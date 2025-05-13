using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Data References")]
    public LevelData levelData;
    public LevelsConfig levelsConfig;

    [Header("Player & Goal")]
    public Transform player;
    public Transform start;
    public Transform goal;
    public PlayerMovementScript playerMovementScript;

    [Header("Oxygen Settings")]
    public float oxygenTime = 2700f;

    [Header("Game Settings")]
    public float goalRadius = 2f;
    public float returnDelay = 4f;

    // internal
    PlayerControls controls;
    bool levelFailed;
    bool levelComplete;

    void Awake()
    {
        // set up Interact for skipping
        controls = new PlayerControls();
        controls.Player.Enable();
    }

    void Start()
    {
        // spawn/orient
        if (player != null && start != null)
        {
            player.position = start.position;
            player.rotation = start.rotation;
        }

        // run intro sequence
        StartCoroutine(PlaySequence(
            levelData.GetInitialLines(),
            levelData.initialTextDismissTime,
            OnIntroComplete));
    }

    public void OnIntroComplete()
    {
        // start normal gameplay (nothing special needed)
    }

        public void FailLevel()
    {
        levelFailed = true;
        StartCoroutine(PlaySequence(
            levelData.GetFailLines(),
            levelData.failTextDismissTime,
            () => StartCoroutine(ReturnToMenu())));
    }

    public void CompleteLevel()
    {
        levelComplete = true;

        // // unlock next - TODO: Implment when additional levels are added and saving is important
        // int idx = SceneManager.GetActiveScene().buildIndex;
        // if (!levelsConfig.tutorialLevels.Contains(idx))
        // {
        //     int highest = SaveManager.currentData.highestUnlockedLevel;
        //     if (idx >= highest)
        //     {
        //         SaveManager.currentData.highestUnlockedLevel = idx + 1;
        //         SaveManager.SaveGame();
        //     }
        // }

        StartCoroutine(PlaySequence(
            levelData.GetSuccessLines(),
            levelData.successTextDismissTime,
            () => StartCoroutine(ReturnToMenu())));
    }

    void Update()
    {
        if (levelFailed || levelComplete) return;

        // deplete oxygen underwater
        if (player.position.y < playerMovementScript.surfaceLevel)
            oxygenTime -= Time.deltaTime;

        if (oxygenTime <= 0f)
            FailLevel();

        // check goal
        if (Vector3.Distance(player.position, goal.position) <= goalRadius)
            CompleteLevel();
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(returnDelay);
        // disable player controls
        controls.Player.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Plays a series of popup lines, each shown for 'dur' or until Interact.
    /// Calls onComplete() after the last line.
    /// </summary>
    IEnumerator PlaySequence(string[] lines, float dur, System.Action onComplete)
    {
        foreach (var line in lines)
        {
            // ensure previous button press is released
            yield return new WaitUntil(() => !controls.Player.Interact.IsPressed());

            // show via speech bubble
            SpeechBubbleUI.Instance.Show(line, dur);

            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                if (controls.Player.Interact.WasPressedThisFrame())
                    break;
                yield return null;
            }
        }

        onComplete?.Invoke();
    }
}
