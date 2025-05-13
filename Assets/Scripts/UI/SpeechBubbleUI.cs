using UnityEngine;
using TMPro;
using System.Collections;

public class SpeechBubbleUI : MonoBehaviour
{
    public static SpeechBubbleUI Instance;

    [Header("Refs")]
    public RectTransform root;
    public CanvasGroup   cg;
    public TextMeshProUGUI body;

    [Header("Animation")]
    public float slideTime = 0.35f;
    public float defaultDuration = 5f;

    Vector2 hiddenPos;     // off-screen (right)
    Vector2 shownPos;      // prefab position
    Coroutine routine;

    PlayerControls controls;

    // Notes if the text is currently showing
    public bool IsShowing => routine != null;

    void Awake()
    {
        Instance = this;

        shownPos  = root.anchoredPosition;          // where you placed it
        hiddenPos = shownPos + new Vector2(+800, 0); // <-- slide in from RIGHT
        root.anchoredPosition = hiddenPos;
        cg.alpha = 0f;

        controls = new PlayerControls();
        controls.Player.Enable();
    }

    void OnDestroy() => controls.Disable();

    public void Show(string msg, float dur = -1f)
    {
        if (dur <= 0f) dur = defaultDuration;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Run(msg, dur));
    }

    IEnumerator Run(string msg, float seconds)
    {
        body.text = msg;

        // slide-in
        for (float t = 0; t < slideTime; t += Time.unscaledDeltaTime)
        {
            float k = t / slideTime;
            root.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, k);
            cg.alpha = k;
            yield return null;
        }
        root.anchoredPosition = shownPos; cg.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            // skip if player presses Interact
            if (controls.Player.Interact.WasPressedThisFrame()) break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // slide-out
        for (float t = 0; t < slideTime; t += Time.unscaledDeltaTime)
        {
            float k = t / slideTime;
            root.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, k);
            cg.alpha = 1f - k;
            yield return null;
        }
        root.anchoredPosition = hiddenPos; cg.alpha = 0f;
        routine = null;
    }
}
