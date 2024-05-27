using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum MyColor
{
    Red,
    Green,
    Blue
}

public class LaserBeam2D : MonoBehaviour
{
    public float speed = 15f; // Speed of the laser beam
    public List<MyColor> laserColors; // List of colors for the laser beam
    public float offset = 0.01f; // Small offset to prevent sticking to walls
    public static event Action OnHitEnemy;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Vector2 direction;
    private int currentColorIndex = 0;
    private List<Vector3> positions = new List<Vector3>();
    private bool isInvincible = false;
    
    
    void Start()
    {
        if (laserColors.Count == 0)
        {
            Debug.LogError("Please add at least one color to the laserColors list.");
            return;
        }

        direction = transform.up;
        positions.Add(transform.position);

        CreateNewLineRenderer();
    }

    void Update()
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + speed * Time.deltaTime * direction;

        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, speed * Time.deltaTime);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                HitEnemy(hit.collider.gameObject);
                return;
            }
            else if (hit.collider.CompareTag("Player"))
            {
                GameController.instance.LoseGame();
                return;
            }

            endPosition = hit.point;
            ReflectLaser(hit.normal);

            // Apply a small offset to the new position to prevent getting stuck
            endPosition += direction * offset;

            // Add the end position of the current segment
            positions.Add(endPosition);
            currentColorIndex = (currentColorIndex + 1) % laserColors.Count;
            CreateNewLineRenderer();

            // Reset positions for the new segment
            positions.Clear();
            positions.Add(endPosition);
        }

        positions.Add(endPosition);
        UpdateCurrentLineRenderer();

        transform.position = endPosition;
    }

    private void ReflectLaser(Vector2 hitNormal)
    {
        direction = Vector2.Reflect(direction, hitNormal);
    }

    private void CreateNewLineRenderer()
    {
        GameObject newLineRendererObj = new GameObject("LaserSegment");
        newLineRendererObj.transform.SetParent(transform);

        LineRenderer newLineRenderer = newLineRendererObj.AddComponent<LineRenderer>();
        newLineRenderer.startColor = GetColor(myColor: laserColors[currentColorIndex]);
        newLineRenderer.endColor = GetColor(myColor: laserColors[currentColorIndex]);
        newLineRenderer.positionCount = 0;
        newLineRenderer.startWidth = 0.1f;
        newLineRenderer.endWidth = 0.1f;
        newLineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        lineRenderers.Add(newLineRenderer);
    }

    private Color GetColor(MyColor myColor)
    {
        switch (myColor)
        {
            case MyColor.Red:
                return Color.red;
            case MyColor.Green:
                return Color.green;
            case MyColor.Blue:
                return Color.blue;
            default:
                return Color.white;
        }
    }

    private void UpdateCurrentLineRenderer()
    {
        LineRenderer currentLineRenderer = lineRenderers[lineRenderers.Count - 1];
        currentLineRenderer.positionCount = positions.Count;
        currentLineRenderer.SetPositions(positions.ToArray());
        // Set the sorting layer index
        currentLineRenderer.sortingOrder = 1;
    }

    private void HitEnemy(GameObject enemyObj)
    {
        if (enemyObj.GetComponent<Enemy>().GetColor() == laserColors[currentColorIndex])
        {
            OnHitEnemy?.Invoke();
            GameController.instance.player.transform.position = enemyObj.transform.position;
            Destroy(enemyObj);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}