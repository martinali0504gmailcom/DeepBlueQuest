using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RockGlue : InteractableBase
{
    [Header("Setup")]
    public GameObject babyCoralPrefab;
    public Transform spawnPoint;
    public LevelData levelData;

    [Header("Completion range (e.g. 1-10 or 2,4,6-8)")]
    public string completionRange = "1-10";

    static int blurbGroupIndex = 0; //next blurb group to show

    bool coralPlaced;

    public bool HasCoralPlaced => coralPlaced;

    //get player controls

    public override void Interact(PlayerInventory inv)
    {
        if (coralPlaced) return;

        if (!inv.HasToolEquipped(ToolType.CoralGlue))
        {
            SpeechBubbleUI.Instance.Show("You need coral glue!", 2f);
            return;
        }

        if (!inv.ConsumeSample())
        {
            SpeechBubbleUI.Instance.Show("You have no coral samples! Go get one!", 2f);
            return;
        }

        Instantiate(babyCoralPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        coralPlaced = true;

        StartCoroutine(InfoThenCongrats());
    }
    IEnumerator InfoThenCongrats()
    {
        // 1) ------------- ordered blurb set -------------
        if (levelData.infoBlurbs != null &&
            blurbGroupIndex < levelData.infoBlurbs.Length)
        {
            var group = levelData.infoBlurbs[blurbGroupIndex++];
            foreach (string line in group.lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                SpeechBubbleUI.Instance.Show(line, levelData.infoBlurbDismissTime);
                yield return WaitForBubbleOrSkip(levelData.infoBlurbDismissTime);
            }
            yield return new WaitForSeconds(0.15f); // small gap
        }

        // 2) ------------- random completion cheer -------------
        string cheer = levelData.GetRandomTaskCompletionMessage();
        if (!string.IsNullOrEmpty(cheer))
            SpeechBubbleUI.Instance.Show(cheer, levelData.taskCompletionDismissTime);

        // 3) ------------- notify objective tracker -------------
        LevelObjectiveTracker.Instance.NotifyCoralGlued(this);
    }

IEnumerator WaitForBubbleOrSkip(float dur)
{
    var c = InputManager.Controls;              // shared instance

    // require button to be released first so a held press doesn't skip twice
    yield return new WaitUntil(() => !c.Player.Interact.IsPressed());

    float t = 0f;
    while (t < dur && SpeechBubbleUI.Instance.IsShowing)
    {
        t += Time.deltaTime;
        if (c.Player.Interact.WasPressedThisFrame()) break;
        yield return null;
    }
}


    // helper
    List<int> ParseRange(string s)
    {
        var l = new List<int>();
        if (string.IsNullOrWhiteSpace(s)) return l;
        foreach (var p in s.Split(','))
        {
            if (p.Contains("-"))
            {
                var ends = p.Split('-');
                if (int.TryParse(ends[0], out int a) && int.TryParse(ends[1], out int b))
                    for (int n = a; n <= b; n++) l.Add(n);
            }
            else if (int.TryParse(p, out int v)) l.Add(v);
        }
        return l;
    }
}
