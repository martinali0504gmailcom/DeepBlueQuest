using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Shows the Clipper or Glue info pop-ups once per game session,
/// no matter how many corals / rocks exist in the level.
/// </summary>
public class ProximityInfoTrigger : MonoBehaviour
{
    public enum TriggerType { Clippers, Glue }
    public TriggerType triggerType = TriggerType.Clippers;

    public float triggerRadius = 3f;
    public LevelData levelData;
    public PlayerInventory playerInv;

    // -------- static once-per-session flags --------
    static bool clipperInfoShown;
    static bool glueInfoShown;

    // -------- instance --------
    bool localFired;
    PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();
    }

    void Update()
    {
        if (localFired || playerInv == null || levelData == null) return;

        // has this info already been shown anywhere?
        if ((triggerType == TriggerType.Clippers  && clipperInfoShown) ||
            (triggerType == TriggerType.Glue      && glueInfoShown))
        {
            localFired = true; // mark so we stop checking distance
            return;
        }

        if (Vector3.Distance(playerInv.transform.position, transform.position) <= triggerRadius)
        {
            localFired = true;
            StartCoroutine(RunSequence());
        }
    }

    IEnumerator RunSequence()
    {
        ToolType tool   = (triggerType == TriggerType.Clippers)
                          ? ToolType.CoralClippers
                          : ToolType.CoralGlue;

        (string txt0, float dur0) = levelData.GetToolLines(tool, 0); // real-life
        (string txt1, float dur1) = levelData.GetToolLines(tool, 1); // gameplay

        SpeechBubbleUI.Instance.Show(txt0, dur0);

        float t = 0f;
        while (t < dur0)
        {
            t += Time.deltaTime;
            if (controls.Player.Interact.WasPressedThisFrame()) break;
            yield return null;
        }

        SpeechBubbleUI.Instance.Show(txt1, dur1);
        yield return new WaitForSeconds(dur1);

        // record globally so no other trigger shows again
        if (triggerType == TriggerType.Clippers) clipperInfoShown = true;
        else                                     glueInfoShown    = true;
    }

    void OnDisable() => controls.Disable();

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerType == TriggerType.Clippers ? Color.cyan : Color.magenta;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
#endif
}
