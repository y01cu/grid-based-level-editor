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
                }, endPoint, _tweenDuration)
            )
            .AppendCallback(() =>
            {
                // DOTween.To(() => lineRenderer.GetPosition(lastPositionIndex), x =>
                // {
                //     worldPosition = transform.TransformPoint(x);
                //     lineRenderer.SetPosition(lastPositionIndex, x);
                //     hits = CheckCollision(startPoint, worldPosition);
                // }, endPoint, _tweenDuration);

                // for (int i = 0; i < hits.Length; i++)
                // {
                //     RaycastHit hit = hits[i];
                //
                //     if (!_detectedObjects.Contains(hit.collider.gameObject))
                //     {
                //         _detectedObjects.Add(hit.collider.gameObject);
                //         
                //         // DOTween.To( hit.collider.transform.position, x =>
                //         // {
                //         //     
                //         // })
                //         // AddPointToLine(hit.point);
                //     }
                //
                //     hits[i].collider.isTrigger = false;
                // }


                // if (hits.Length > 0)
                // {
                //     Berry lastBerry = hits[hits.Length - 1].collider.GetComponent<Berry>();
                //     if (lastBerry != null)
                //     {
                //         lastBerry.SetDirection(-(worldPosition - startPoint) * 20);
                //         lastBerry.MoveToFrog();
                //     }
                // }
                //
                // Debug.Log("started callback 0");


                // CheckCollision(startPoint, worldPosition);
            })
            // .AppendInterval(_interval).AppendCallback(() =>
            // {
            //     Debug.Log("started callback 1");
            //
            //     for (int i = 0; i < lineRenderer.positionCount; i++)
            //     {
            //         Vector3 targetPosition = startPoint;
            //         int index = i;
            //
            //         DOTween.To(() => lineRenderer.GetPosition(index), x =>
            //         {
            //             lineRenderer.SetPosition(index, x);
            //             if (index < _detectedObjects.Count)
            //             {
            //                 _detectedObjects[index].transform.position = x;
            //             }
            //         }, targetPosition, _tweenDuration);
            //     }
            // })
            .AppendInterval(_interval).SetLoops(2, LoopType.Yoyo).onComplete += () =>
        {
            _isTongueOutside = false;

            FreeBerriesForFrog?.Invoke();
        };
    }

    private void AddPointToLine(Vector3 point)
    {
        int currentCount = lineRenderer.positionCount;
        lineRenderer.positionCount = currentCount + 1;
        lineRenderer.SetPosition(currentCount, point);
    }

    // private void TweenBackToNormal()
    // {
    // }

    private RaycastHit[] JustCheckCollision(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = endPoint - startPoint;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        Debug.Log("leeength: " + hits.Length);
        
        Debug.DrawRay(startPoint, direction, Color.cyan, .5f);

        lineRenderer.positionCount = hits.Length + 1;
        
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
                    _detectedObjects.Add(hitBerry.gameObject);
                    
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