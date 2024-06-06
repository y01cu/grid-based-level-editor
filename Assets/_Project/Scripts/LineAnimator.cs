using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    [SerializeField] private float animationDuration;

    [SerializeField] private LineRenderer lineRenderer;
    private Vector3[] _linePoints;
    private int _pointsCount;

    private void Start()
    {
        // Store a copy of LineRenderer's points.

        _pointsCount = lineRenderer.positionCount;
        _linePoints = new Vector3[_pointsCount];

        for (int i = 0; i < _pointsCount; i++)
        {
            _linePoints[i] = lineRenderer.GetPosition(i);
        }

        StartCoroutine(AnimateLine());
    }

    private IEnumerator AnimateLine()
    {
        float segmentDuration = animationDuration / _pointsCount;

        for (int i = 0; i < _pointsCount - 1; i++)
        {
            float startTime = Time.time;

            Vector3 startPosition = _linePoints[i];
            Vector3 endPosition = _linePoints[i + 1];

            Vector3 position = startPosition;
            while (position != endPosition)
            {
                float timer = (Time.time - startTime) / segmentDuration;
                position = Vector3.Lerp(startPosition, endPosition, timer);

                for (int j = i + 1; j < _pointsCount; j++)
                {
                    lineRenderer.SetPosition(j, position);
                }

                yield return null;
            }
        }
    }
}