using UnityEngine;
using System.Collections;
public class BasicFishAI : MonoBehaviour
{

    [Header("Movement")]
    [Tooltip("Swim Speed")]
    public float swimSpeed = 3f;
    [Tooltip("How close to a waypoint before the fish chooses a new waypoint")]
    public float waypointThreshold = 1.5f;
    [Tooltip("Does the Fish randonly choose a point to move to, or goes in order?")]
    public bool randomWaypoints = true;


    [Header("Avoidance Settings")]
    [Tooltip("Distance for raycast checker")]
    public float obstacleCheckDistance = 2.5f;
    [Tooltip("Speed mult to turn from obstacles")]
    public float avoidanceTurnSpeed = 2f;
    [Header("Player Avoidance Settings")]
    [Tooltip("Distance at which fish flees from player")]
    public float fleeDistance = 4.5f;
    [Tooltip("Flee speed multiplier")]
    public float fleeSpeedMultiplier = 2f;
    [Tooltip("Time till AI reverts from fleeing")]
    public float fleeDuration = 3f;
    [Header("References")]
    [Tooltip("Player Transform to avoid")]
    public Transform playerTransform;
    [Tooltip("List of preset waypoints (positions) the fish will swim between")]
    public Transform[] waypoints;

    //This fish has two states: move from point to point, and swimming away from something
    private enum FishState { Patrol, Flee }

    private FishState currentState = FishState.Patrol;

    private int currentWaypointIndex = 0;
    private float fleeTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (randomWaypoints)
        {
            //Select random waypoint to start
            currentWaypointIndex = Random.Range(0, waypoints.Length);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check to see if player is within flee distance
        float distToPlayer = (playerTransform.position - transform.position).magnitude;
        if (distToPlayer <= fleeDistance)
        {
            //Switch to flee state
            currentState = FishState.Flee;
            fleeTimer = fleeDuration;
        }

        switch(currentState)
        {
            case FishState.Patrol:
                fleeTimer = 0f;
                PatrolBehavior();
                break;
            case FishState.Flee:
                FleeBehavior();
                break;
        }
    }

    void PatrolBehavior()
    {
        //move to current waypoint
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        //obstacle avoidance
        direction = AvoidObstacles(direction);

        //Move in that direction
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * avoidanceTurnSpeed);
        transform.position += transform.forward * (swimSpeed * Time.deltaTime);

        //check if checkpoint is reached
        float distToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distToWaypoint <= waypointThreshold)
        {
            if (randomWaypoints)
            {
                currentWaypointIndex = Random.Range(0, waypoints.Length);
            }
            else
            {
                //move in a loop
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }
    }

    void FleeBehavior()
    {
        //decrement flee timer
        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0f)
        {
            //Return to patrol
            currentState = FishState.Patrol;
            return;
        }

        //move away from player
        Vector3 awayFromPlayer = (transform.position - playerTransform.position).normalized;

        //check for obstacles in direction
        awayFromPlayer = AvoidObstacles(awayFromPlayer);

        //turn and move
        transform.forward = Vector3.Lerp(transform.forward, awayFromPlayer, Time.deltaTime * avoidanceTurnSpeed);
        transform.position += transform.forward * (swimSpeed * fleeSpeedMultiplier * Time.deltaTime);
    }

    //If there is an obstalce, we try to avoid
    //Simple solution: Shoot raycast. If it hit anything, rotate away from the hit normal
    Vector3 AvoidObstacles(Vector3 desiredDirection)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, obstacleCheckDistance))
        {
            //if we hit something, turn away
            Vector3 hitNormal = hit.normal;
            desiredDirection = Vector3.Reflect(desiredDirection, hitNormal);
        }
        return desiredDirection.normalized;
    }

}
