using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LineCollision lineCollision;

    [SerializeField] private BoxCollider tongueBoxCollider;

    private bool isLineOutside;
    // private bool isObstacleHit;

    public bool IsLineOutside
    {
        get => isLineOutside;
    }

    private void Start()
    {
        lineCollision.detectedObjectStorage.points = new List<Vector3>();
        lineCollision.detectedObjectStorage.points.Add(lineRenderer.GetPosition(0));
        SetUpLineRenderer();
    }

    private void SetUpLineRenderer()
    {
        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;
        lineRenderer.positionCount = 1;
    }

    public void MoveTongueLine(Vector3 startPoint, Vector3 direction, CellGeneration.OrderType orderType, CellBase.ObjectColor objectColor)
    {
        var temp = lineCollision.JustCheckCollisionReturnIfHit(startPoint, direction, orderType, objectColor);

        Debug.Log($"is obs hit: {lineCollision.IsObstacleHit}");

        Debug.Log($"detected obj count: {lineCollision.detectedObjectStorage.detectedObjects.Count}");

        if (lineCollision.detectedObjectStorage.detectedObjects.Count > 0)
        {
            if (lineCollision.IsObstacleHit)
            {
                Debug.Log("------------");
            }
            else if (!lineCollision.IsObstacleHit)
            {
                var newPoint = transform.parent.InverseTransformPoint(lineCollision.detectedObjectStorage.detectedObjects[^1].transform.localPosition);
                lineCollision.detectedObjectStorage.points.Add(newPoint);
                Debug.Log($"added point: l48{newPoint} | is obs hit: {lineCollision.IsObstacleHit}");
            }
        }

        Debug.Log(lineCollision.detectedObjectStorage.detectedBerries.Count);

        AnimateLine();
    }

    public LineRenderer GetLineRenderer()
    {
        return lineRenderer;
    }

    public void AnimateLine()
    {
        Debug.Log("started animating line");
        Sequence sequence = DOTween.Sequence();

        // Sequence sequence = DOTween.Sequence();
        isLineOutside = true;
        float totalDuration = 1.5f;

        float segmentDuration = totalDuration / lineCollision.detectedObjectStorage.points.Count;

        // segmentDuration = totalDuration / points.Count;

        Debug.Log("segment duration: " + segmentDuration);
        // Animate forward
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, lineCollision.detectedObjectStorage.points[0]);
        }

        RaycastHit[] hits = null;

        for (int i = 1; i < lineCollision.detectedObjectStorage.points.Count; i++)
        {
            int index = i;
            Vector3 start = lineCollision.detectedObjectStorage.points[index - 1];
            Vector3 end = lineCollision.detectedObjectStorage.points[index];

            Debug.Log($"start: {start} | end: {end}");

            sequence.Append(DOTween.To(() => start, x =>
            {
                UpdateLine(index, x);
                if (tongueBoxCollider != null)
                {
                    tongueBoxCollider.center = x;
                }
            }, end, segmentDuration).SetEase(Ease.Linear));
        }


        sequence.AppendCallback(() =>
        {
            // var isFoundLastBerry = !isObstacleHit && detectedObjectStorage.detectedBerries[^1].IsLastBerryForFrog();
            if (!lineCollision.IsObstacleHit && lineCollision.detectedObjectStorage.detectedBerries[^1].IsLastBerryForFrog())
            {
                if (tongueBoxCollider != null)
                {
                    lineCollision.detectedObjectStorage.detectedBerries[^1].SetTargetBoxCollider(tongueBoxCollider);
                }

                if (lineRenderer != null)
                {
                    lineCollision.detectedObjectStorage.detectedBerries[^1].SetLineRenderer(lineRenderer, segmentDuration);
                }
            }
            else
            {
                // obstacle hit
                var lastDetectedObject = lineCollision.detectedObjectStorage.detectedObjects[^1];
                lastDetectedObject.GetComponent<CellObject>().HandleBeingObstacle();
                Debug.Log($"this is the obstacle obj {lastDetectedObject} | obs hit state: {lineCollision.IsObstacleHit}", lastDetectedObject);
            }
        });

        // Animate backward
        for (int i = lineCollision.detectedObjectStorage.points.Count - 1; i > 0; i--)
        {
            int index = i;
            Vector3 end = lineCollision.detectedObjectStorage.points[index - 1];
            Vector3 start = lineCollision.detectedObjectStorage.points[index];

            sequence.Append(DOTween.To(() => start, x =>
            {
                if (tongueBoxCollider != null)
                {
                    tongueBoxCollider.center = x;
                }

                UpdateLine(index, x);
            }, end, segmentDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (lineRenderer != null)
                {
                    lineRenderer.positionCount--;
                    if (index == 1)
                    {
                        lineRenderer.positionCount = 1; // Reset to one point to complete the backward animation
                    }
                }
            }));
        }

        sequence.onComplete += () => { ResetBackToInitialState(); };
    }

    private void UpdateLine(int index, Vector3 position)
    {
        if (lineRenderer != null)
        {
            if (lineRenderer.positionCount < index + 1)
            {
                lineRenderer.positionCount = index + 1;
            }

            lineRenderer.SetPosition(index, position);
        }
    }

    public List<Berry> GetDetectedBerries()
    {
        return lineCollision.detectedObjectStorage.detectedBerries;
    }

    private void ResetBackToInitialState()
    {
        Debug.Log("reset back");
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