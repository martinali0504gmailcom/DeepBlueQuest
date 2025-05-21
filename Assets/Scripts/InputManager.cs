using UnityEngine;
public class InputManager : MonoBehaviour
{
    public static PlayerControls Controls { get; private set; }

    void Awake()
    {
        if (Controls != null) { Destroy(gameObject); return; }  // already initialised

        Controls = new PlayerControls();
        Controls.Player.Enable();        // enable the whole action map

        DontDestroyOnLoad(gameObject);   // persists to every scene
    }
}