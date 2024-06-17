using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Serialization;


public class Frog : Clickable
{
    public event Action FreeBerriesForFrog;
    public event Action MoveBerriesToFrog;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LayerMask collisionMask;

    private bool _isTongueOutside;

    [SerializeField] private TongueNew newTongue;

    [SerializeField] private float time;

    [SerializeField] private BoxCollider boxCollider;

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

    private void Start()
    {
        gameObject.name = properNaming.GetProperName();

        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;

        points.Add(lineRenderer.GetPosition(0));
    }

    public override void OnClickedOver()
    {
        if (_isTongueOutside)
        {
            return;
        }

        base.OnClickedOver();

        lineRenderer.positionCount = 1;

        Vector3 startPoint = transform.TransformPoint(lineRenderer.GetPosition(0));
        Vector3 rotation = transform.parent.localRotation.eulerAngles;


        Vector3 direction = (int)rotation.y switch
        {
            0 => Vector3.up,
            180 => Vector3.down,
            90 => Vector3.right,
            270 => Vector3.left,
        };

        Debug.Log("start point: " + startPoint + " | dir: " + direction);

        JustCheckCollision(startPoint, direction);

        if (berryPoints.Count > 0)
        {
            points.Add(berryPoints[berryPoints.Count - 1]);
        }


        // Debug.Log("berry point: " + berryPoints[berryPoints.Count - 1]);

        for (int i = 0; i < points.Count; i++)
        {
            // points[i] = transform.TransformPoint(points[i]);
            Debug.Log("point val: " + points[i]);
        }

        // TweenBetweenTwoPoints(startPoint);

        time = berryCounter * 2.5f;
        AnimateLine();
    }

    void AnimateLine()
    {
        Sequence sequence = DOTween.Sequence();

        // Animate forward
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, points[0]);

        RaycastHit[] hits = null;

        for (int i = 1; i < points.Count; i++)
        {
            int index = i;
            Vector3 start = points[index - 1];
            Vector3 end = points[index];

            sequence.Append(DOTween.To(() => start, x =>
            {
                // CheckCollision(x, Vector3.up);
                UpdateLine(index, x);
                boxCollider.center = x;

                // boxCollider.
                // Berry hitBerry = hits[i].collider.GetComponent<Berry>();
                // FreeBerriesForFrog += hitBerry.SetAsHitable;
            }, end, time / points.Count).SetEase(Ease.Linear));
            //.AppendCallback(() => { detectedBerries[^1].SetLineRenderer(lineRenderer); });
        }

        sequence.AppendCallback(() =>
        {
            detectedBerries[^1].SetTargetBoxCollider(boxCollider);
            detectedBerries[^1].SetLineRenderer(lineRenderer);
        });


        // Animate backward
        for (int i = points.Count - 1; i > 0; i--)
        {
            int index = i;
            Vector3 end = points[index - 1];
            Vector3 start = points[index];

            sequence.Append(DOTween.To(() => start, x =>
            {
                boxCollider.center = x;
                UpdateLine(index, x);
            }, end, time / points.Count).SetEase(Ease.Linear).OnComplete(() =>
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

        var newPosition = position;

        lineRenderer.SetPosition(index, newPosition);
    }

    private bool _isTongueReachedEnd;

    private float _tweenDuration = 1.2f;
    private float _interval = 0.15f;

    public List<Berry> detectedBerries = new();

    public List<Berry> GetDetectedObjects()
    {
        return detectedBerries;
    }

    // This function below can be made async

    private bool _isReachedEnd;

    private List<Vector3> points = new();

    private List<Vector3> berryPoints = new();

    // private void TweenBetweenTwoPoints(Vector3 startPoint)
    // {
    //     _isTongueOutside = true;
    //
    //     Vector3 worldPosition = Vector3.zero;
    //
    //     RaycastHit[] hits = null;
    //
    //     Sequence mySequence = DOTween.Sequence();
    //
    //     Vector3 rotation = transform.parent.localRotation.eulerAngles;
    //
    //     Vector3 direction = rotation.x switch
    //     {
    //         0 => Vector3.right,
    //         90 => Vector3.down,
    //         270 => Vector3.up,
    //     };
    //
    //     Vector3 endPoint = JustCheckCollision(startPoint, direction);
    //
    //     int lastPositionIndex = lineRenderer.positionCount - 1;
    //
    //     mySequence.Append(
    //             DOTween.To(() => lineRenderer.GetPosition(lastPositionIndex), x =>
    //             {
    //                 worldPosition = transform.TransformPoint(x);
    //                 lineRenderer.SetPosition(lastPositionIndex, x);
    //                 hits = CheckCollision(startPoint, worldPosition);
    //             }, endPoint, _tweenDuration)
    //         )
    //         .AppendInterval(_interval).AppendCallback(() =>
    //         {
    //             Debug.Log("reached end");
    //
    //             detectedBerries[^1].GetComponent<Berry>().SetLineRenderer(lineRenderer);
    //         })
    //         // .Append(DOTween.To(()=> 0f, x =>
    //         // {
    //         //     for (int i = 0; i < lineRenderer.positionCount; i++)
    //         //     {
    //         //         Vector3 targetPosition = Vector3.Lerp(lineRenderer.GetPosition(i), startPoint, x);
    //         //         lineRenderer.SetPosition(i, targetPosition);
    //         //         
    //         //         
    //         //     }
    //         // }, 1f, _tweenDuration))
    //         .SetLoops(2, LoopType.Yoyo)
    //         .onComplete += () =>
    //     {
    //         _isTongueOutside = false;
    //
    //         FreeBerriesForFrog?.Invoke();
    //     };
    // }

    private int berryCounter = 0;

    private Vector3 JustCheckCollision(Vector3 startPoint, Vector3 direction)
    {
        // Vector3 direction = endPoint - startPoint;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        Debug.DrawRay(startPoint, direction, Color.cyan, 2f);

        // lineRenderer.positionCount = hits.Length + 1;

        // for (int i = 0; i < hits.Length; i++)
        // {
        //     _detectedObjects.Add(hits[i].collider.gameObject);
        // }

        bool isAnyArrowHit = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider currentCollider = hits[i].collider;

            if (currentCollider.CompareTag("Arrow"))
            {
                // TODO: The multiplier must change based on arrow rotation

                Debug.Log("arrow pos: " + currentCollider.transform.position);
                points.Add(new Vector3(transform.localPosition.x, 0, MathF.Abs((int)transform.position.y - currentCollider.transform.localPosition.y)));


                isAnyArrowHit = true;

                Arrow.Direction arrowDirection = currentCollider.GetComponent<Arrow>().direction;

                switch (arrowDirection)
                {
                    case Arrow.Direction.Left:
                        Debug.Log("la");
                        JustCheckCollision(currentCollider.transform.position, -Vector3.right);
                        break;
                    case Arrow.Direction.Right:
                        JustCheckCollision(currentCollider.transform.position, Vector3.right);
                        Debug.Log("ra");
                        break;
                    case Arrow.Direction.Up:
                        JustCheckCollision(currentCollider.transform.position, Vector3.up);
                        Debug.Log("ua");
                        break;
                    case Arrow.Direction.Down:
                        JustCheckCollision(currentCollider.transform.position, Vector3.down);
                        Debug.Log("da");
                        break;

                    default:
                        Debug.Log("la");
                        break;
                }
            }

            if (currentCollider.CompareTag("Berry"))
            {
                // currentCollider.transform.SetParent(transform);

                Vector3 relativePosition = transform.parent.TransformPoint(currentCollider.transform.position);
                Debug.Log("this pos: " + transform.parent.position + "| child pos: " + transform.position + " | relative position: " + relativePosition + " | current pos: " + currentCollider.transform.position + "current loc pos: " + currentCollider.transform.localPosition, currentCollider);

                // berryPoints.Add(new Vector3(currentCollider.transform.localPosition.x - transform.parent.position.x, 0, (int)currentCollider.transform.localPosition.y));
                // berryPoints.Add(new Vector3(currentCollider.transform.localPosition.x - transform.parent.position.x, 0, MathF.Abs((int)transform.position.y - currentCollider.transform.localPosition.y)));


                var xRotation = transform.parent.localRotation.eulerAngles.x;
                if (xRotation == 90 || xRotation == -90 || xRotation == 270)
                {
                    // Frog up or down
                    berryPoints.Add(new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - currentCollider.transform.localPosition.y)));
                    // berryPoints.Add(new Vector3(transform.localPosition.x, 0, MathF.Abs((int)transform.position.y - currentCollider.transform.localPosition.y)));
                }
                else
                {
                    berryPoints.Add(new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.x - currentCollider.transform.localPosition.x)));
                    // berryPoints.Add(new Vector3(transform.localPosition.x, 0, MathF.Abs((int)transform.position.x - currentCollider.transform.localPosition.x)));
                }


                Debug.Log("berry collision", currentCollider);
                berryCounter++;
                Berry berry = currentCollider.GetComponent<Berry>();
                if (berry.isLastForFrog)
                {
                    // Stop tongue movement
                }
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit && berryCounter < CellGeneration.Instance.GetHeight())
        {
            JustCheckCollision(startPoint + direction, direction);
        }

        else
        {
            Debug.Log("total detected berry count: " + berryCounter);
        }


        return new Vector3(0, 0, berryCounter);
    }

    // private RaycastHit[] CheckCollision(Vector3 startPoint, Vector3 dir)
    // {
    //     // Debug.DrawLine(startPoint, endPoint, Color.blue, 1f);
    //
    //     // Vector3 direction = endPoint - startPoint;
    //
    //     float distance = dir.magnitude;
    //
    //     // This detection process takes time.
    //
    //     RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, distance, collisionMask);
    //
    //
    //     // Debug.DrawLine(startPoint, dir, Color.magenta, 2f);
    //
    //
    //     for (int i = 0; i < hits.Length; i++)
    //     {
    //         if (hits[i].collider.CompareTag("Arrow"))
    //         {
    //             Vector3 startPosition = hits[i].collider.transform.position;
    //             lineRenderer.positionCount++;
    //             // lineRenderer.SetPosition(lineRenderer.positionCount-1, );
    //
    //             return CheckCollision(startPosition, new Vector3(startPosition.x - 1, startPosition.y, startPosition.z));
    //         }
    //
    //         if (hits[i].collider.CompareTag("Berry"))
    //         {
    //             // Berry hitBerry = hits[i].collider.GetComponent<Berry>();
    //             // FreeBerriesForFrog += hitBerry.SetAsHitable;
    //             // hitBerry.OnClickedOver();
    //             // hitBerry.SetTongueHit();
    //
    //             // hitBerry.SetDirection(-(direction));
    //             // MoveBerriesToFrog += hitBerry.MoveToFrog;
    //
    //             // if (!detectedBerries.Contains(hitBerry))
    //             // {
    //             //     // lineRenderer.SetPosition(_detectedObjects.Count + 1, new Vector3(0, 0, lineRenderer.GetPosition(lineRenderer.positionCount - 1).z));
    //             //
    //             //     detectedBerries.Add(hitBerry);
    //             //     // DOTween.To( hit.collider.transform.position, x =>
    //             //     // {
    //             //     //     
    //             //     // })
    //             //     // AddPointToLine(hit.point);
    //             // }
    //         }
    //     }
    //
    //     // if ()
    //     // {
    //     // Debug.Log("frog color: " + _colorFirstLetter + " hit color: " + hit.collider.gameObject.name[0]);
    //
    //     // TODO: There could be performance improvements here
    //
    //
    //     // }
    //
    //     return hits;
    // }
}