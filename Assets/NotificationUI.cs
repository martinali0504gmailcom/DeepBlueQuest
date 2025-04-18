using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The TMP text that will display messages")]
    public TextMeshProUGUI notificationText;
    // We'll store a reference to any running coroutine
    private Coroutine hideCoroutine;

    void Awake()
    {
        // Hide text on start
        if (notificationText)
            notificationText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show a message for 'displayDuration' seconds, then hide automatically.
    /// </summary>
    public void ShowMessage(string message, float durationToShow = 3f)
    {
        if (!notificationText) return;

        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        // If a hide coroutine is still running, stop it so we don't cut off new messages early
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterSeconds(durationToShow));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        notificationText.gameObject.SetActive(false);
        hideCoroutine = null;
    }
}
