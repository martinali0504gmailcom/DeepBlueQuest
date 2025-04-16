using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    [Tooltip("Which build indexes are tutorials? These levels are always avaliable for the player, and do not effect progression.")]
    public int[] tutorialLevels;
}
