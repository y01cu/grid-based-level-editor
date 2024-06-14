using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueNew : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float tongueSpeed = 1f; // Speed of the tongue movement
    private Vector3 startPosition = Vector3.zero; // The start position of the tongue (usually the frog's mouth)
    private List<Vector3> tonguePositions = new List<Vector3>();

    void Start()
    {
        // Initialize the Line Renderer positions
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
    }

    public void OnMouseRightClick()
    {
        // Start the tongue movement coroutine
        StartCoroutine(MoveTongue());
    }

    IEnumerator MoveTongue()
    {
        Vector3 currentPos = startPosition;
        tonguePositions.Clear();
        tonguePositions.Add(currentPos);

        bool collecting = true;

        while (collecting)
        {
            // Calculate the next position
            Vector3 nextPos = GetNextTonguePosition(currentPos);

            // Update the Line Renderer
            tonguePositions.Add(nextPos);
            lineRenderer.positionCount = tonguePositions.Count;
            lineRenderer.SetPositions(tonguePositions.ToArray());

            // Move the current position
            currentPos = nextPos;

            // Check if the tongue should stop (based on your game logic, e.g., reaching a berry)
            if (CheckIfStopCondition(currentPos))
            {
                collecting = false;
            }

            // Wait for the next frame
            yield return null;
        }

        // Retract the tongue
        StartCoroutine(RetractTongue());
    }

    Vector3 GetNextTonguePosition(Vector3 currentPos)
    {
        // Implement your logic to determine the next position of the tongue
        // For example, move in the current direction until a certain condition is met
        return currentPos - transform.up * tongueSpeed * Time.deltaTime;
    }

    bool CheckIfStopCondition(Vector3 currentPos)
    {
        // Implement your logic to check if the tongue should stop moving
        // For example, if it reaches a berry or an arrow
        return false;
    }

    IEnumerator RetractTongue()
    {
        while (tonguePositions.Count > 1)
        {
            // Remove the last position
            tonguePositions.RemoveAt(tonguePositions.Count - 1);
            lineRenderer.positionCount = tonguePositions.Count;
            lineRenderer.SetPositions(tonguePositions.ToArray());

            // Wait for the next frame
            yield return null;
        }
    }
}
