using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Slides a single speech-bubble on / off screen. Portrait sprite is set
/// once in the prefab and reused for every call.
/// </summary>
public class SpeechBubbleUI : MonoBehaviour
{
    public static SpeechBubbleUI Instance;

    [Header("Refs")]
    public RectTransform root;         // SpeechBubble RectTransform
    public CanvasGroup    cg;          // for fade
    public TextMeshProUGUI body;       // TMP text field

    [Header("Animation")]
    public float slideTime = 0.35f;    // seconds
    public float defaultDuration = 5f; // auto-hide delay

    Vector2 hiddenPos;  // off-screen
    Vector2 shownPos;   // on-screen
    Coroutine routine;

    void Awake()
    {
        Instance = this;

        shownPos  = root.anchoredPosition;          // prefab’s position
        hiddenPos = shownPos + new Vector2(-800, 0); // slide in from left
        root.anchoredPosition = hiddenPos;
        cg.alpha = 0f;
    }

    /// <summary>Display text for given seconds (or defaultDuration).</summary>
    public void Show(string message, float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Run(message, duration));
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

        // wait
        yield return new WaitForSecondsRealtime(seconds);

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
