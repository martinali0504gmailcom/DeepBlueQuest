using UnityEngine;
using UnityEngine.Rendering; //Uses Volume

public class UnderwaterVolumeController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the player's script that has 'surfaceLevel' and transform")]
    public PlayerMovementScript playerMovement;

    [Tooltip("Post-processing volume that handles underwater effects.")]
    public Volume underwaterVolume;

    [Header("Transition Settings")]
    [Tooltip("If > 0, smoothly blend the effect in/out over time rather than instantly. The smaller the number the slower the transition.")]
    public float transitionSpeed = 1f;

    private float targetWeight = 0f; //Where we want volume.weight to go


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Set it off by default above water
        if (underwaterVolume != null)
        {
            underwaterVolume.weight = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null || underwaterVolume == null)
            return;
        //Compare the player's Y-pos versus the defined surface level
        float depth = playerMovement.transform.position.y - playerMovement.surfaceLevel;

        //if Depth < 0, we are underwater, and thus weight = 1, otherwise weight = 0
        if (depth < 0f)
        {
            targetWeight = 1f;
        }
        else
        {
            targetWeight = 0f;
        }

        //Smooth transition settings
        if (transitionSpeed > 0f)
        {
            underwaterVolume.weight = Mathf.MoveTowards(
                underwaterVolume.weight,
                targetWeight,
                transitionSpeed * Time.deltaTime
            );
        }
        else
        {
            //instant toggle
            underwaterVolume.weight = targetWeight;
        }
    }
}
