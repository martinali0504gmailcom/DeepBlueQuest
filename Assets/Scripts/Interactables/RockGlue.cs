using System.Collections.Generic;
using UnityEngine;

public class RockGlue : InteractableBase
{
    [Header("Setup")]
    public GameObject babyCoralPrefab;
    public Transform spawnPoint;
    public LevelData levelData;

    [Header("Completion range (e.g. 1-10 or 2,4,6-8)")]
    public string completionRange = "1-10";

    bool coralPlaced;

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

        // random completion line
        string msg = levelData.GetRandomTaskCompletionMessage();
        if (!string.IsNullOrEmpty(msg))
            SpeechBubbleUI.Instance.Show(msg, levelData.taskCompletionDismissTime);

        LevelObjectiveTracker.Instance.NotifyCoralGlued(this);
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
