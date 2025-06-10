using System.Collections.Generic;
using UnityEngine;

public class BirdPathFollower : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 3f;
    public float rotationSpeed = 5f;
    private int currentWaypoint = 0;

    void Start()
    {
        // Optional: Auto-remove Rigidbody if added accidentally
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            Destroy(rb);
        }
    }

    void Update()
    {
        if (waypoints.Count == 0 || currentWaypoint >= waypoints.Count) return;
        Transform target = waypoints[currentWaypoint];

        // Movement (includes Y-axis)
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Rotation (horizontal only)
        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero)
        {
            direction.y = 0; // Lock vertical rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Advance waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}