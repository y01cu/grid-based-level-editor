using System;
using DG.Tweening;
using UnityEngine;

public class LineAnimation
{
    private readonly LineRenderer lineRenderer;
    private readonly BoxCollider tongueBoxCollider;
    private readonly LineCollision lineCollision;
    private readonly Action onComplete;

    public LineAnimation(LineRenderer lineRenderer, BoxCollider tongueBoxCollider, LineCollision lineCollision, Action onComplete)
    {
        this.lineRenderer = lineRenderer;
        this.tongueBoxCollider = tongueBoxCollider;
        this.lineCollision = lineCollision;
        this.onComplete = onComplete;
    }

    public void AnimateLine()
    {
        Sequence sequence = DOTween.Sequence();

        float totalDuration = 1.5f;
        float segmentDuration = totalDuration / lineCollision.detectedObjectStorage.points.Count;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, lineCollision.detectedObjectStorage.points[0]);

        for (int i = 1; i < lineCollision.detectedObjectStorage.points.Count; i++)
        {
            int index = i;
            Vector3 start = lineCollision.detectedObjectStorage.points[index - 1];
            Vector3 end = lineCollision.detectedObjectStorage.points[index];

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
            if (!lineCollision.IsObstacleHit && lineCollision.detectedObjectStorage.detectedBerries[^1].IsLastBerryForFrog())
            {
                var lastBerry = lineCollision.detectedObjectStorage.detectedBerries[^1];
                lastBerry.SetTargetBoxCollider(tongueBoxCollider);
                lastBerry.SetLineRenderer(lineRenderer, segmentDuration);
            }
            else
            {
                var lastDetectedObject = lineCollision.detectedObjectStorage.detectedObjects[^1];
                lastDetectedObject.GetComponent<CellObject>().HandleBeingObstacle();
            }
        });

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
                lineRenderer.positionCount--;
                if (index == 1)
                {
                    lineRenderer.positionCount = 1; // Reset to one point to complete the backward animation
                }
            }));
        }

        sequence.onComplete += () => onComplete();
    }

    private void UpdateLine(int index, Vector3 position)
    {
        if (lineRenderer.positionCount < index + 1)
        {
            lineRenderer.positionCount = index + 1;
        }

        lineRenderer.SetPosition(index, position);
    }
}