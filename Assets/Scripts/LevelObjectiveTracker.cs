using UnityEngine;

public class LevelObjectiveTracker : MonoBehaviour
{
    public static LevelObjectiveTracker Instance;

    [Header("References")]
    public LevelManager levelManager;      // drag the LevelManager in scene
    public CoralClipping[] corals;         // fill via inspector (or FindObjectsOfType)
    public RockGlue[]     rocks;           // same length & order as corals

    private int snippedCount = 0;
    private int gluedCount   = 0;

    void Awake() { Instance = this; }

    public void NotifyCoralSnipped(CoralClipping c)
    {
        snippedCount++;
        CheckWin();
    }

    public void NotifyCoralGlued(RockGlue r)
    {
        gluedCount++;
        CheckWin();
    }

    void CheckWin()
    {
        if (snippedCount == corals.Length && gluedCount == rocks.Length)
        {
            levelManager.OnLevelComplete();   // triggers success popup + save
        }
    }
}
