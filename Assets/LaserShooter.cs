using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] private GameObject laserBeamPrefab;
    [SerializeField] private float laserSpeed = 10f; // Desired speed of the laser beam
    
    public static GameObject player;
    LineRenderer aimLine;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        aimLine = GetComponent<LineRenderer>();
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
        
        // Move the player 
        float moveSpeed = 5f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, vertical, 0);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
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
}