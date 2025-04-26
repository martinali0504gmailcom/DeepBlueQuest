using UnityEngine;

[System.Serializable]
public class RangeString
{
    [Tooltip("E.g., 1-4, 6, 8-10")]
    public string range = "1-10";
}

[CreateAssetMenu(fileName = "LevelData", menuName = "DeepBlueQuest/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName;

    [TextArea]
    public string initialText; //text shown at the start of the game
    public float initialTextDismissTime = 7f;

    [TextArea]
    public string failText; //text shown if player fails the level
    public float failTextDismissTime = 10f;

    [TextArea]
    public string successText; //text shown if player completes the level
    public float successTextDismissTime = 10f;

    [Header("Tool Info.")]
    [Tooltip("Information given to the player about the tools and how they work. The first box is explaining the real-life application of the tool, the second explains how to use them in game.")]
    [TextArea]
    public string clipperInfoText1; //Real-World context
    [TextArea]
    public string clipperInfoText2; //Gameplay Instructions
    public float clipperInfoTextDismissTime = 10f;

    [TextArea]
    public string vaccumInfoText1; //Real-World context
    [TextArea]
    public string vaccumInfoText2; //Gameplay Instructions
    public float vaccumInfoTextDismissTime = 10f;

    [TextArea]
    public string glueInfoText1; //Real-World context
    [TextArea]
    public string glueInfoText2; //Gameplay Instructions
    public float glueInfoTextDismissTime = 10f;

    //Repetable messages - text used when the player completes part of the level, choosen at random.
    [Header("Repetable Messages")]
    [Tooltip("Messages that will pop up when the player completes a task. The message is chosen from the following non-blank options at random.")]
    [TextArea]
    public string taskCompletionText1; 
    [TextArea]
    public string taskCompletionText2;
    [TextArea]
    public string taskCompletionText3; 
    [TextArea]
    public string taskCompletionText4;
    [TextArea]
    public string taskCompletionText5; 
    [TextArea]
    public string taskCompletionText6;
    [TextArea]
    public string taskCompletionText7; 
    [TextArea]
    public string taskCompletionText8;
    [TextArea]
    public string taskCompletionText9; 
    [TextArea]
    public string taskCompletionText10;
    public float taskCompletionTextDismissTime = 5f;

    public (string, float) GetToolLines(ToolType tool, int stage)
    {
        switch (tool)
        {
            case ToolType.CoralClippers:
                return stage == 0
                    ? (clipperInfoText1, clipperInfoTextDismissTime)
                    : (clipperInfoText2, clipperInfoTextDismissTime);

            case ToolType.CoralGlue:
                return stage == 0
                    ? (glueInfoText1, glueInfoTextDismissTime)
                    : (glueInfoText2, glueInfoTextDismissTime);
        }
        return ("", 4f);
    }

    public string GetRandomCompletionLine(System.Collections.Generic.List<int> allowed)
    {
        string[] pool = {
            taskCompletionText1, taskCompletionText2, taskCompletionText3,
            taskCompletionText4, taskCompletionText5, taskCompletionText6,
            taskCompletionText7, taskCompletionText8, taskCompletionText9,
            taskCompletionText10
        };

        var valid = new System.Collections.Generic.List<string>();
        foreach (int idx in allowed)
            if (idx >= 1 && idx <= pool.Length && !string.IsNullOrWhiteSpace(pool[idx-1]))
                valid.Add(pool[idx-1]);

        if (valid.Count == 0) return "";
        return valid[Random.Range(0, valid.Count)];
    }
}
