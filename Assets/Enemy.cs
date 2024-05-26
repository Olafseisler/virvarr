using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private MyColor enemyColor;
    [SerializeField] private bool isMoving = false;
    [SerializeField] List<Transform> waypoints = new List<Transform>();

    private int _wayPointIndex = 0;
    private Transform nextWaypoint;
    private bool movingForward = true;
    
    public MyColor GetColor()
    {
        return enemyColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the sprite color
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (enemyColor)
        {
            case MyColor.Red:
                spriteRenderer.color = Color.red;
                break;
            case MyColor.Green:
                spriteRenderer.color = Color.green;
                break;
            case MyColor.Blue:
                spriteRenderer.color = Color.blue;
                break;
        }
        
        if (isMoving)
            nextWaypoint = waypoints[_wayPointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
            return;

        // Move the enemy towards the next waypoint
        // If the enemy reaches the last waypoint, start moving backwards through the waypoints
        Vector3 currentPosition = transform.position;
        Vector3 direction = (nextWaypoint.position - currentPosition).normalized;
        transform.position += direction * Time.deltaTime;

        // Check if the enemy has reached the next waypoint
        if (Vector3.Distance(currentPosition, nextWaypoint.position) < 0.1f)
        {
            nextWaypoint = waypoints[_wayPointIndex++];
            if (_wayPointIndex >= waypoints.Count)
            {
                _wayPointIndex = 0;
                waypoints.Reverse();
            }
        }
    }
}