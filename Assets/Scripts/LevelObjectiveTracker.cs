using UnityEngine;

public class LevelObjectiveTracker : MonoBehaviour
{
    public static LevelObjectiveTracker Instance;
    [Header("References")]
    public NotificationUI notificationSystem;

    [Header("References")]
    public LevelManager levelManager;      // drag the LevelManager in scene
    public CoralClipping[] corals;         // fill via inspector (or FindObjectsOfType)
    public RockGlue[]     rocks;           // same length & order as corals
    public float timeToDisplayNotif = 2f;

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
        if (gluedCount == rocks.Length) //Only care about making sure every rock than can be glued, is.s
        {
            levelManager.CompleteLevel();   // triggers success popup + save
        }
        else {
            // // show progress when planting coral (and only when planting coral)
            // if (snippedCount > 0 && snippedCount < corals.Length)
            // {
            //    string msg = $"Coral planted: {snippedCount}/{corals.Length}";
            //    notificationSystem.ShowMessage(msg, 2f);
            //}
            //else if (gluedCount > 0 && gluedCount < rocks.Length)
            //{
            //    string msg = $"Coral glued: {gluedCount}/{rocks.Length}";
            //    notificationSystem.ShowMessage(msg, 2f);
            //}
            MessagePlayer();
        }
    }

    void MessagePlayer()
    {
        string msg = $"Coral Planted: {gluedCount}/{rocks.Length}";
        notificationSystem.ShowMessage(msg, timeToDisplayNotif);
    }
}
