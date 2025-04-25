using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PushBackWalls : MonoBehaviour
{
    [Tooltip("Speed (in m/s) that the wall pushes the player back")]
    public float pushForce = 8f;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) 
        {
            Debug.Log("Error, not player!");
            return;
        }

        Vector3 dir = (other.transform.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        dir = dir.normalized;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.Move(dir * pushForce * Time.deltaTime);
        }
        else {
            Debug.Log("Error, no CC grabbed!");
        }

        
    }
}
