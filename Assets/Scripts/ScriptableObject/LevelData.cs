using UnityEngine;

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
}
