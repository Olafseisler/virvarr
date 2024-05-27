using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] private float moveForce = 5f;
    [SerializeField] private GameObject laserBeamPrefab;
    [SerializeField] private float laserSpeed = 10f; // Desired speed of the laser beam

    public static GameObject player;
    private LineRenderer aimLine;
    private bool touchingWall = false;
    private Rigidbody2D rb;
    private Animator _animator;
    private bool _isInvincible = false;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        rb = GetComponent<Rigidbody2D>();
        aimLine = GetComponent<LineRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        LaserBeam2D.OnHitEnemy += PlayTeleportAnimation;
    }

    private void OnDisable()
    {
        LaserBeam2D.OnHitEnemy -= PlayTeleportAnimation;
    }

    private void PlayTeleportAnimation()
    {
        _animator.SetTrigger("Teleported");
    }

    public void ToggleInvincible()
    {
        Debug.Log("Toggling invincibility");
        if (gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Point the laser beam in the direction of the mouse (2D)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        transform.up = direction;
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);

        // Shoot the laser beam
        if (Input.GetMouseButtonDown(0))
        {
            GameObject laserBeam = Instantiate(laserBeamPrefab, transform.position, transform.rotation);
            Destroy(laserBeam, 2f);
            StartCoroutine(ToggleAimLine());
        }

        DrawAimLine(direction);
    }

    private void FixedUpdate()
    {
        // Move the player's rigidbody
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.AddForce(movement * moveForce);
    }

    // Draw aimline to the nearest wall
    private void DrawAimLine(Vector3 direction)
    {
        // Ignore the player's collider
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity);
        // Draw a linereneder to the nearest wall
        if (hit.collider != null)
        {
            LineRenderer aimLine = GetComponent<LineRenderer>();
            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, hit.point);
        }
        else // Draw a linerenderer to the end of the screen
        {
            LineRenderer aimLine = GetComponent<LineRenderer>();
            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, transform.position + direction * 100);
        }
    }

    // Turn on/off the aimline for a period
    private IEnumerator ToggleAimLine()
    {
        aimLine.enabled = false;
        yield return new WaitForSeconds(1f);
        aimLine.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        touchingWall = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        touchingWall = false;
    }
}