// LevelData.cs – now provides individual dismiss times per tool
using UnityEngine;

[System.Serializable]
public class MultiLineBlurb
{
    [TextArea] public string[] lines;   // each element is one speech‑bubble in order
}

[System.Serializable]
public class RangeString { public string range = "1-10"; }

[CreateAssetMenu(fileName = "LevelData", menuName = "DeepBlueQuest/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    // ---------- LEVEL FLOW ----------
    [Header("Intro / Fail / Success")]
    [TextArea] public string[] initialMessages;  public float initialTextDismissTime = 7f;
    [TextArea] public string[] failMessages;     public float failTextDismissTime    = 10f;
    [TextArea] public string[] successMessages;  public float successTextDismissTime = 10f;

    // ---------- TOOL INFO ----------
    [Header("Clippers (real‑life / gameplay)")]
    [TextArea] public string[] clipperInfoLines1;  // real‑life
    [TextArea] public string[] clipperInfoLines2;  // gameplay
    public float clipperInfoDismissTime = 8f;

    [Header("Vacuum (real‑life / gameplay)")]
    [TextArea] public string[] vaccumInfoLines1;
    [TextArea] public string[] vaccumInfoLines2;
    public float vaccumInfoDismissTime = 8f;

    [Header("Glue (real‑life / gameplay)")]
    [TextArea] public string[] glueInfoLines1;
    [TextArea] public string[] glueInfoLines2;
    public float glueInfoDismissTime = 8f;

    // ---------- TASK COMPLETIONS ----------
    [Header("Task Completion Pool (random)")]
    [TextArea] public string[] taskCompletionMessages;
    public float taskCompletionDismissTime = 5f;

    // ---------- INFO BLURBS that appear after each glue ----------
    [Header("Ordered Info Blurbs (each element can be multi‑line)")]
    public MultiLineBlurb[] infoBlurbs;        // element #N will be shown after glue #N
    public float infoBlurbDismissTime = 6f;


    // ---------- HELPERS ----------
    public string[] GetInitialLines()  => initialMessages;
    public string[] GetFailLines()     => failMessages;
    public string[] GetSuccessLines()  => successMessages;

    public string[] GetToolLines(ToolType tool, int stage)
    {
        switch (tool)
        {
            case ToolType.CoralClippers: return stage == 0 ? clipperInfoLines1 : clipperInfoLines2;
            case ToolType.CoralGlue:     return stage == 0 ? glueInfoLines1    : glueInfoLines2;
            default:                     return new string[0];
        }
    }

    public float GetToolDismissTime(ToolType tool)
    {
        return tool switch
        {
            ToolType.CoralClippers => clipperInfoDismissTime,
            ToolType.CoralGlue     => glueInfoDismissTime,
            _                      => 3f
        };
    }

    public string GetRandomTaskCompletionMessage()
    {
        var valid = System.Array.FindAll(taskCompletionMessages, m => !string.IsNullOrWhiteSpace(m));
        if (valid.Length == 0) return string.Empty;
        return valid[Random.Range(0, valid.Length)];
    }
}