using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class LineRendererAnimator : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float time = 1f;

    private List<Vector3> points = new List<Vector3>();

    private void Start()
    {
        // Store the initial points of the line
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            points.Add(lineRenderer.GetPosition(i));
        }

        // Reset the line renderer to have no points initially
        lineRenderer.positionCount = 0;

        // Start the animation
        AnimateLine();
    }

    void AnimateLine()
    {
        Sequence sequence = DOTween.Sequence();

        // Animate forward
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, points[0]);

        for (int i = 1; i < points.Count; i++)
        {
            int index = i;
            Vector3 start = points[index - 1];
            Vector3 end = points[index];

            sequence.Append(DOTween.To(() => start, x => UpdateLine(index, x), end, time / points.Count).SetEase(Ease.Linear));
        }

        // Animate backward
        for (int i = points.Count - 1; i > 0; i--)
        {
            int index = i;
            Vector3 end = points[index - 1];
            Vector3 start = points[index];

            sequence.Append(DOTween.To(() => start, x => UpdateLine(index, x), end, time / points.Count).SetEase(Ease.Linear).OnComplete(() =>
            {
                lineRenderer.positionCount--;
                if (index == 1)
                {
                    lineRenderer.positionCount = 1; // Reset to one point to complete the backward animation
                }
            }));
        }

        sequence.AppendCallback(() => lineRenderer.positionCount = 0); // Reset to no points
    }

    void UpdateLine(int index, Vector3 position)
    {
        if (lineRenderer.positionCount < index + 1)
        {
            lineRenderer.positionCount = index + 1;
        }
        lineRenderer.SetPosition(index, position);
    }
}
