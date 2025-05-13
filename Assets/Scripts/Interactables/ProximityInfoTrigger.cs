// ProximityInfoTrigger.cs – use per‑tool dismiss time
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ProximityInfoTrigger : MonoBehaviour
{
    public enum TriggerType { Clippers, Glue }
    public TriggerType triggerType = TriggerType.Clippers;

    public float triggerRadius = 3f;
    public LevelData levelData;
    public PlayerInventory playerInv;

    static bool clipperInfoShown;
    static bool glueInfoShown;

    bool localFired;
    PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();
    }

    void Update()
    {
        if (localFired || levelData == null || playerInv == null) return;
        if ((triggerType == TriggerType.Clippers && clipperInfoShown) || (triggerType == TriggerType.Glue && glueInfoShown))
        { localFired = true; return; }
        if (Vector3.Distance(playerInv.transform.position, transform.position) <= triggerRadius)
        {
            localFired = true;
            StartCoroutine(RunSequence());
        }
    }

    IEnumerator RunSequence()
    {
        ToolType tool = triggerType == TriggerType.Clippers ? ToolType.CoralClippers : ToolType.CoralGlue;
        float dismiss = levelData.GetToolDismissTime(tool);

        string[] set1 = levelData.GetToolLines(tool, 0);
        string[] set2 = levelData.GetToolLines(tool, 1);

        foreach (var line in set1)
        {
            SpeechBubbleUI.Instance.Show(line, dismiss);
            yield return WaitOrSkip(dismiss);
        }
        foreach (var line in set2)
        {
            SpeechBubbleUI.Instance.Show(line, dismiss);
            yield return WaitOrSkip(dismiss);
        }

        if (triggerType == TriggerType.Clippers) clipperInfoShown = true; else glueInfoShown = true;
    }

    IEnumerator WaitOrSkip(float dur)
    {
        yield return new WaitUntil(() => !controls.Player.Interact.IsPressed());
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            if (controls.Player.Interact.WasPressedThisFrame()) yield break;
            yield return null;
        }
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
