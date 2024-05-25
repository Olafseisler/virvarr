using UnityEngine;
using System.Collections.Generic;

public class LaserBeam2D : MonoBehaviour
{
    public float speed = 10f; // Speed of the laser beam
    public List<Color> laserColors; // List of colors for the laser beam
    public float offset = 0.01f; // Small offset to prevent sticking to walls

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Vector2 direction;
    private int currentColorIndex = 0;
    private List<Vector3> positions = new List<Vector3>();

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
        Vector2 endPosition = startPosition + direction * speed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, speed * Time.deltaTime);
        if (hit.collider != null)
        {
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
        newLineRendererObj.transform.SetParent(this.transform);

        LineRenderer newLineRenderer = newLineRendererObj.AddComponent<LineRenderer>();
        newLineRenderer.startColor = laserColors[currentColorIndex];
        newLineRenderer.endColor = laserColors[currentColorIndex];
        newLineRenderer.positionCount = 0;
        newLineRenderer.startWidth = 0.1f; 
        newLineRenderer.endWidth = 0.1f; 
        newLineRenderer.material = new Material(Shader.Find("Sprites/Default")); 

        lineRenderers.Add(newLineRenderer);
    }

    private void UpdateCurrentLineRenderer()
    {
        LineRenderer currentLineRenderer = lineRenderers[lineRenderers.Count - 1];
        currentLineRenderer.positionCount = positions.Count;
        currentLineRenderer.SetPositions(positions.ToArray());
    }
    
}
