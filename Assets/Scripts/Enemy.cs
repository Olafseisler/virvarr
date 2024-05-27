using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum EnemyBehavior
    {
        Stationary,
        Patrol,
        Attack
    }

    [SerializeField] private MyColor enemyColor;
    [SerializeField] List<Transform> waypoints = new();
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float moveSpeed = 3f;

    private int _wayPointIndex = 0;
    private Transform nextWaypoint;
    private EnemyBehavior _enemyBehavior = EnemyBehavior.Stationary;
    private bool movingForward = true;
    private Vector3 _restPosition;

    public MyColor GetColor()
    {
        return enemyColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        _restPosition = transform.position;
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

        if (waypoints.Count > 0)
            nextWaypoint = waypoints[_wayPointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        var gcInstance = GameController.instance;
        // If player is within a certain distance, change the enemy behavior to attack
        if (gcInstance && Vector2.Distance(transform.position, gcInstance.player.transform.position) < attackRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                LaserShooter.player.transform.position - transform.position);
            _enemyBehavior = hit.collider.CompareTag("Player") ? EnemyBehavior.Attack :
                waypoints.Count > 0 ? EnemyBehavior.Patrol : EnemyBehavior.Stationary;
        }
        else
        {
            _enemyBehavior = waypoints.Count > 0 ? EnemyBehavior.Patrol : EnemyBehavior.Stationary;
        }

        if (_enemyBehavior == EnemyBehavior.Patrol)
        {
            Patrol();
        }

        if (_enemyBehavior == EnemyBehavior.Attack)
        {
            Attack();
        }

        if (_enemyBehavior == EnemyBehavior.Stationary)
        {
            // Move to the rest position
            if (Vector3.Distance(transform.position, _restPosition) > 0.1f)
            {
                Vector3 direction = (_restPosition - transform.position).normalized;
                transform.position += direction * Time.deltaTime;
            }
        }
    }

    void Patrol()
    {
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

    void Attack()
    {
        // Move the enemy towards the player
        // If the enemy reaches the player, deal damage to the player
        Vector2 dirToPlayer = LaserShooter.player.transform.position - transform.position;
        transform.up = dirToPlayer;
        transform.position += (Vector3)dirToPlayer.normalized * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameController.instance.LoseGame();
        }
    }
}