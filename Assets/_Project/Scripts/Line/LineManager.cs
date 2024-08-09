using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public bool IsLineOutside => isLineOutside;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineCollision lineCollision;
    [SerializeField] private BoxCollider tongueBoxCollider;

    private bool isLineOutside;

    private LineAnimation lineAnimation;

    private void Start()
    {
        lineCollision.detectedObjectStorage.points = new List<Vector3>() { lineRenderer.GetPosition(0) };
        SetUpLineRenderer();
        lineAnimation = new LineAnimation(lineRenderer, tongueBoxCollider, lineCollision, ResetBackToInitialState);
    }

    private void SetUpLineRenderer()
    {
        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;
        lineRenderer.positionCount = 1;
    }

    public void MoveTongueLine(Vector3 startPoint, Vector3 direction, string frogColor)
    {
        lineCollision.JustCheckCollision(startPoint, direction, frogColor);

        if (lineCollision.detectedObjectStorage.detectedObjects.Count > 0)
        {
            if (!lineCollision.IsObstacleHit)
            {
                var newPoint = transform.parent.InverseTransformPoint(lineCollision.detectedObjectStorage.detectedObjects[^1].transform.localPosition);
                lineCollision.detectedObjectStorage.points.Add(newPoint);
            }
        }

        isLineOutside = true;
        lineAnimation.AnimateLine();
    }

    public LineRenderer GetLineRenderer()
    {
        return lineRenderer;
    }

    public List<Berry> GetDetectedBerries()
    {
        return lineCollision.detectedObjectStorage.detectedBerries;
    }

    private void ResetBackToInitialState()
    {
        foreach (var berry in lineCollision.detectedObjectStorage.detectedBerries)
        {
            berry.TurnBackToNormalState();
        }

        lineCollision.detectedObjectStorage.detectedBerries.Clear();
        lineCollision.detectedObjectStorage.detectedObjects.Clear();
        lineCollision.detectedObjectStorage.points.Clear();

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.localPosition);
        lineCollision.detectedObjectStorage.points.Add(lineRenderer.GetPosition(0));

        isLineOutside = false;
        lineCollision.IsObstacleHit = false;
    }
}