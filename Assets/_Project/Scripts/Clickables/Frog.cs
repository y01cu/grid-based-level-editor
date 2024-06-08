using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor.Build.Content;
using UnityEngine;


public class Frog : Clickable
{
    public event Action FreeBerriesForFrog;
    public event Action MoveBerriesToFrog;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LayerMask collisionMask;

    private bool _isTongueOutside;

    // Objects are named as first with their color then with their type
    // Colors and namings are:
    // B:Blue
    // G:Green
    // R:Red
    // Y:Yellow
    // B_B: Blue Berry
    // R_A: Red Arrow
    // G_F: Green Frog

    // _colorFirstLetter = transform.parent.gameObject.name[0];

    // private char _colorFirstLetter;

    public override void OnClickedOver()
    {
        if (_isTongueOutside)
        {
            return;
        }

        base.OnClickedOver();


        // increase z by one for normal direction

        // lineRenderer.SetPosition(lineRenderer.positionCount - 1, lineRenderer.GetPosition(lineRenderer.positionCount - 1) + new Vector3(0, 0, 1));

        // ---

        // Check if path is clear

        Vector3 startPoint = transform.TransformPoint(lineRenderer.GetPosition(0));
        Vector3 endPoint = new Vector3(0, 0, 3);

        TweenBetweenTwoPoints(startPoint, endPoint);
    }

    private bool _isTongueReachedEnd;

    private float _tweenDuration = 1.2f;
    private float _interval = 0.15f;

    private List<GameObject> _detectedObjects = new();

    // This function below can be made async

    private bool _isReachedEnd;

    private void TweenBetweenTwoPoints(Vector3 startPoint, Vector3 endPoint)
    {
        _isTongueOutside = true;


        Vector3 worldPosition = Vector3.zero;

        RaycastHit[] hits = null;

        Sequence mySequence = DOTween.Sequence();

        // Ray direction change condition must be considered here.
        JustCheckCollision(startPoint, transform.TransformPoint(endPoint));

        int lastPositionIndex = lineRenderer.positionCount - 1;

        mySequence.Append(
                DOTween.To(() => lineRenderer.GetPosition(lastPositionIndex), x =>
                {
                    worldPosition = transform.TransformPoint(x);
                    lineRenderer.SetPosition(lastPositionIndex, x);
                    hits = CheckCollision(startPoint, worldPosition);

                    // for (int i = 0; i < lineRenderer.positionCount; i++)
                    // {
                    //     lineRenderer.SetPosition(i, x);
                    // }   

                    // if (!_isReachedEnd)
                    // {
                    // }

                    // There could a check added here
                    // for (int i = 0; i < _detectedObjects.Count; i++)
                    // {
                    //     _detectedObjects[i].transform.position = lineRenderer.GetPosition(i + 1);
                    // }
                }, endPoint, _tweenDuration)
            )
            .AppendInterval(_interval).AppendCallback(() =>
            {
                Debug.Log("reached end");
                for (int i = 0; i < hits.Length; i++)
                {
                    hits[i].collider.isTrigger = false;
                }
            })
            // .Append(DOTween.To(()=> 0f, x =>
            // {
            //     for (int i = 0; i < lineRenderer.positionCount; i++)
            //     {
            //         Vector3 targetPosition = Vector3.Lerp(lineRenderer.GetPosition(i), startPoint, x);
            //         lineRenderer.SetPosition(i, targetPosition);
            //         
            //         
            //     }
            // }, 1f, _tweenDuration))
            .SetLoops(2, LoopType.Yoyo)
            .onComplete += () =>
        {
            _isTongueOutside = false;

            FreeBerriesForFrog?.Invoke();
        };
    }

    private RaycastHit[] JustCheckCollision(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = endPoint - startPoint;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        Debug.Log("leeength: " + hits.Length);

        Debug.DrawRay(startPoint, direction, Color.cyan, .5f);

        // lineRenderer.positionCount = hits.Length + 1;

        // for (int i = 0; i < hits.Length; i++)
        // {
        //     _detectedObjects.Add(hits[i].collider.gameObject);
        // }

        return hits;
    }

    private RaycastHit[] CheckCollision(Vector3 startPoint, Vector3 endPoint)
    {
        // Debug.DrawLine(startPoint, endPoint, Color.blue, 1f);

        Vector3 direction = endPoint - startPoint;

        float distance = direction.magnitude;

        // This detection process takes time.

        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        // Debug.Log("--- hits length: " + hits.Length);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.name[0] == properNaming.GetColor())
            {
                Berry hitBerry = hits[i].collider.GetComponent<Berry>();
                hitBerry.OnClickedOver();
                hitBerry.SetTongueHit();
                FreeBerriesForFrog += hitBerry.SetAsHitable;

                // hitBerry.SetDirection(-(direction));
                // MoveBerriesToFrog += hitBerry.MoveToFrog;

                if (!_detectedObjects.Contains(hitBerry.gameObject))
                {
                    // lineRenderer.SetPosition(_detectedObjects.Count + 1, new Vector3(0, 0, lineRenderer.GetPosition(lineRenderer.positionCount - 1).z));

                    _detectedObjects.Add(hitBerry.gameObject);
                    hitBerry.SetLineRenderer(lineRenderer);
                    // DOTween.To( hit.collider.transform.position, x =>
                    // {
                    //     
                    // })
                    // AddPointToLine(hit.point);
                }
            }
        }

        // if ()
        // {
        // Debug.Log("frog color: " + _colorFirstLetter + " hit color: " + hit.collider.gameObject.name[0]);

        // TODO: There could be performance improvements here


        // }

        return hits;
    }
}