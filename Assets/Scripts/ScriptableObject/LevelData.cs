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
}
