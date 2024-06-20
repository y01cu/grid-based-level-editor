using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Schema;
using DG.Tweening;
using QFSW.QC;
using UnityEditor;
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

    private CellGeneration.OrderType orderType;

    [SerializeField] private bool isObstacleHit;

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

    private Sequence sequence;

    public bool IsTongueOutside()
    {
        return _isTongueOutside;
    }

    private void Start()
    {
        gameObject.name = properNaming.GetProperName();

        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;

        points.Add(lineRenderer.GetPosition(0));
    }

    public void SetOrderType(CellGeneration.OrderType newOrderType)
    {
        orderType = newOrderType;
    }

    public override async void OnClickedOver()
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
            90 => Vector3.right,
            180 => Vector3.down,
            270 => Vector3.left,
        };

        Debug.Log("start point: " + startPoint + " | dir: " + direction);

        JustCheckCollision(startPoint, direction);

        if (berries.Count > 0)
        {
            // Vector3 relativePosition = cube2.transform.InverseTransformPoint(worldPosition);


            // berries[^1].transform.SetParent(lineRenderer.transform);

            // Debug.Log("last berry: " + berries[^1].transform.localPosition);

            points.Add(transform.parent.InverseTransformPoint(berries[^1].transform.localPosition));
            Debug.Log("added last berry point: " + points[^1]);
        }

        // Debug.Log("berry point: " + berryPoints[berryPoints.Count - 1]);

        // TweenBetweenTwoPoints(startPoint);

        time = berryCounter * 2.5f;
        AnimateLine();
    }


    void AnimateLine()
    {
        sequence = DOTween.Sequence();

        // Sequence sequence = DOTween.Sequence();
        _isTongueOutside = true;
        float totalDuration = 1.5f;

        segmentDuration = totalDuration / points.Count;

        Debug.Log("segment duration: " + segmentDuration);
        // Animate forward
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, points[0]);
        }

        RaycastHit[] hits = null;

        for (int i = 1; i < points.Count; i++)
        {
            int index = i;
            Vector3 start = points[index - 1];
            Vector3 end = points[index];

            var distance = (start - end);

            int distanceValue = (int)MathF.Abs((distance.x + distance.y + distance.z));


            // Debug.Log("start: " + start + " - end: " + end);
            // Debug.Log("subs of start and end: " + distance);
            // Debug.Log("distance value: " + distanceValue);

            sequence.Append(DOTween.To(() => start, x =>
            {
                if (!isObstacleHit)
                {
                    // CheckCollision(x, Vector3.up);
                    UpdateLine(index, x);
                    if (boxCollider != null)
                    {
                        boxCollider.center = x;
                    }
                }

                // boxCollider.
                // Berry hitBerry = hits[i].collider.GetComponent<Berry>();
                // FreeBerriesForFrog += hitBerry.SetAsHitable;
            }, end, segmentDuration).SetEase(Ease.Linear));
            // }, end, time / points.Count).SetEase(Ease.Linear));
            //.AppendCallback(() => { detectedBerries[^1].SetLineRenderer(lineRenderer); });
        }

        sequence.AppendCallback(() =>
        {
            if (!isObstacleHit)
            {
                if (boxCollider != null)
                {
                    detectedBerries[^1].SetTargetBoxCollider(boxCollider);
                }

                if (lineRenderer != null)
                {
                    detectedBerries[^1].SetLineRenderer(lineRenderer, segmentDuration);
                }
            }
        });


        // Animate backward
        for (int i = points.Count - 1; i > 0; i--)
        {
            int index = i;
            Vector3 end = points[index - 1];
            Vector3 start = points[index];

            var distance = (start - end);

            int distanceValue = (int)MathF.Abs((distance.x + distance.y + distance.z));


            sequence.Append(DOTween.To(() => start, x =>
            {
                if (boxCollider != null)
                {
                    boxCollider.center = x;
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

        sequence.AppendCallback(() =>
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
                _isTongueOutside = false;
            }
        }).onComplete += () => { _isTongueOutside = false; }; // Reset to no points
    }

    private void AnimateBackward(Sequence sequence)
    {
        for (int i = points.Count - 1; i > 0; i--)
        {
            int index = i;
            Vector3 end = points[index - 1];
            Vector3 start = points[index];

            var distance = (start - end);

            int distanceValue = (int)MathF.Abs((distance.x + distance.y + distance.z));


            sequence.Append(DOTween.To(() => start, x =>
            {
                if (boxCollider != null)
                {
                    boxCollider.center = x;
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

        sequence.AppendCallback(() =>
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
                _isTongueOutside = false;
            }
        });
    }

    void UpdateLine(int index, Vector3 position)
    {
        if (lineRenderer != null)
        {
            if (lineRenderer.positionCount < index + 1)
            {
                lineRenderer.positionCount = index + 1;
            }

            var newPosition = position;

            lineRenderer.SetPosition(index, newPosition);
        }
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

    private List<Berry> berries = new();
    private int berryCounter = 0;
    private float segmentDuration;

    private void JustCheckCollision(Vector3 startPoint, Vector3 direction)
    {
        // Vector3 direction = endPoint - startPoint;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        if (hits.Length > 0)
        {
            bool isSameColor = hits[0].collider.gameObject.name[0] == gameObject.name[0];
            bool isCollidedWithFrog = hits[0].collider.gameObject.name[2] == 'F';

            if (isSameColor)
            {
                Debug.Log("first letters are same");
            }
            else
            {
                Debug.Log("first letters are not same", hits[0].collider);
                return;
            }

            if (isCollidedWithFrog)
            {
                return;
            }
        }

        Debug.DrawRay(startPoint, direction, Color.cyan, 2f);

        bool isAnyArrowHit = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider currentCollider = hits[i].collider;

            // if (currentCollider.gameObject.name[0] == gameObject.name[0])
            // {
            //     continue;
            // }

            if (currentCollider.CompareTag("Arrow"))
            {
                berryCounter = 0;

                // TODO: The multiplier must change based on arrow rotation

                // Try adding the point here:

                isAnyArrowHit = true;

                Direction arrowDirection = currentCollider.GetComponent<Arrow>().direction;

                var newPoint = Vector3.zero;

                switch (arrowDirection)
                {
                    case Direction.Left:

                        if (orderType == CellGeneration.OrderType.Column)
                        {
                            Debug.Log("parent y rotation: " + (int)transform.parent.localRotation.eulerAngles.y);
                            // TODO: Handle specific index

                            // 0-> up
                            // 90-> right

                            switch ((int)transform.parent.localRotation.eulerAngles.y)
                            {
                                case 0:
                                    // up
                                    Debug.Log("upp--: " + (currentCollider.transform.position.x - transform.parent.position.x));
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("new point of it: " + newPoint);

                                    break;
                                case 90:
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("rightt");
                                    //right
                                    break;
                                case 180:
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("down");
                                    // down
                                    break;
                                case 270:
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("left");
                                    // left
                                    break;
                            }


                            // newPoint = new Vector3((int)currentCollider.transform.position.x - (int)transform.parent.transform.position.x, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));
                        }


                        else
                        {
                            Debug.Log("parent y rotation(row): " + (int)transform.parent.localRotation.eulerAngles.y);
                            // TODO: Handle specific index

                            // 0-> up
                            // 90-> right

                            switch ((int)transform.parent.localRotation.eulerAngles.y)
                            {
                                case 0:
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));

                                    break;
                                case 90:
                                    newPoint = new Vector3((int)transform.parent.position.y - (int)currentCollider.transform.position.y, 0, MathF.Abs((int)transform.position.x - (int)currentCollider.transform.localPosition.x));
                                    //right
                                    break;
                                case 180:
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));
                                    // down
                                    break;
                                case 270:
                                    newPoint = new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));
                                    // left
                                    break;
                            }
                        }

                        points.Add(newPoint);

                        JustCheckCollision(currentCollider.transform.position, Vector3.left);

                        break;
                    case Direction.Right:
                        if (orderType == CellGeneration.OrderType.Column)
                        {
                            // TODO: Handle specific index

                            // 0-> up
                            // 90-> right

                            switch ((int)transform.parent.localRotation.eulerAngles.y)
                            {
                                case 0:
                                    // up
                                    newPoint = new Vector3(currentCollider.transform.position.x - transform.parent.position.x, 0, MathF.Abs((int)transform.parent.position.y - (int)currentCollider.transform.localPosition.y));


                                    break;
                                case 90:
                                    newPoint = new Vector3(currentCollider.transform.position.y - transform.parent.position.y, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));


                                    //right
                                    break;
                                case 180:
                                    newPoint = new Vector3(currentCollider.transform.position.y - transform.parent.position.y, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));


                                    // down
                                    break;
                                case 270:
                                    newPoint = new Vector3(currentCollider.transform.position.y - transform.parent.position.y, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));


                                    // left
                                    break;
                            }
                        }
                        else
                        {
                            newPoint = new Vector3(currentCollider.transform.position.y - transform.parent.position.y, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));
                        }

                        points.Add(newPoint);
                        JustCheckCollision(currentCollider.transform.position, Vector3.right);

                        break;
                    case Direction.Up:
                        newPoint = new Vector3((int)currentCollider.transform.localPosition.y - (int)transform.parent.position.y, 0, MathF.Abs(currentCollider.transform.position.x - transform.parent.position.x));
                        points.Add(newPoint);
                        JustCheckCollision(currentCollider.transform.position, Vector3.up);

                        break;
                    case Direction.Down:

                        if (orderType == CellGeneration.OrderType.Column)
                        {
                            Debug.Log("parent y rotation: " + (int)transform.parent.localRotation.eulerAngles.y);
                            // TODO: Handle specific index

                            // 0-> up
                            // 90-> right

                            switch ((int)transform.parent.localRotation.eulerAngles.y)
                            {
                                case 0:
                                    // up
                                    Debug.Log("upp");
                                    newPoint = new Vector3((int)currentCollider.transform.position.x - (int)transform.parent.transform.position.x, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));
                                    Debug.Log("new point of it: " + newPoint);

                                    break;
                                case 90:
                                    newPoint = new Vector3((int)currentCollider.transform.position.x - (int)transform.parent.transform.position.x, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));

                                    Debug.Log("rightt");
                                    //right
                                    break;
                                case 180:
                                    newPoint = new Vector3((int)currentCollider.transform.position.x - (int)transform.parent.transform.position.x, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));

                                    Debug.Log("down");
                                    // down
                                    break;
                                case 270:
                                    newPoint = new Vector3((int)currentCollider.transform.position.x - (int)transform.parent.transform.position.x, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));

                                    Debug.Log("left");
                                    // left
                                    break;
                            }
                        }
                        else
                        {
                            newPoint = new Vector3(0, 0, MathF.Abs((int)transform.parent.position.x - (int)currentCollider.transform.localPosition.x));
                        }

                        points.Add(newPoint);
                        JustCheckCollision(currentCollider.transform.position, Vector3.down);

                        break;
                }
            }

            if (currentCollider.CompareTag("Berry"))
            {
                // currentCollider.transform.SetParent(transform);

                // Vector3 relativePosition = transform.parent.TransformPoint(currentCollider.transform.position);


                berryCounter++;
                Berry berry = currentCollider.GetComponent<Berry>();
                if (berry.isLastForFrog)
                {
                    // Stop tongue movement
                }

                if (berry.gameObject.name[0] == gameObject.name[0] && !berry.IsDetected())
                {
                    berry.SetAsDetected();

                    Debug.Log("this berry's tongue hit state: " + berry.IsTongueHit(), berry);

                    berries.Add(berry);
                }


                // Debug.Log("berry's relative pos: " + transform.p);


                // var xRotation = transform.parent.localRotation.eulerAngles.x;
                // if (xRotation == 90 || xRotation == -90 || xRotation == 270)
                // {
                //     // Frog up or down
                //     berries.Add(new Vector3(Mathf.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y)));
                //     // berryPoints.Add(new Vector3(transform.localPosition.x, 0, MathF.Abs((int)transform.position.y - currentCollider.transform.localPosition.y)));
                // }
                // else
                // {
                //     if (orderType == CellGeneration.OrderType.Column)
                //     {
                //         var isBerryOnLeftSide = berry.transform.position.x - transform.parent.position.x < 0;
                //     }
                //     else
                //     {
                //         // Is order type row:
                //
                //         var isBerryOnLeftSide = berry.transform.position.x - transform.parent.position.x < 0;
                //     }
                //
                //     Debug.Log("hi | berry pos: " + berry.transform.position);
                //
                //
                //     if (isBerryOnLeftSide)
                //     {
                //         if (orderType == CellGeneration.OrderType.Column)
                //         {
                //             berries.Add(new Vector3((int)transform.parent.position.x - (int)berry.transform.localPosition.x, 0, (int)berry.transform.localPosition.y - (int)transform.parent.localPosition.y));
                //         }
                //         else
                //         {
                //             berries.Add(new Vector3((int)berry.transform.localPosition.y - (int)transform.parent.localPosition.y, 0, MathF.Abs((int)transform.parent.position.x - (int)berry.transform.localPosition.x)));
                //         }
                //     }
                //     else
                //     {
                //         if (orderType == CellGeneration.OrderType.Column)
                //         {
                //             berries.Add(new Vector3((int)transform.parent.localPosition.y - (int)berry.transform.localPosition.y, 0, MathF.Abs((int)berry.transform.localPosition.x - (int)transform.parent.position.x)));
                //         }
                //         else
                //         {
                //             berries.Add(new Vector3((int)transform.parent.localPosition.y - (int)berry.transform.localPosition.y, 0, MathF.Abs((int)berry.transform.localPosition.x - (int)transform.parent.position.x)));
                //
                //
                //             // berryPoints.Add(new Vector3(MathF.Abs((int)berry.transform.localPosition.x - (int)transform.parent.position.x), 0, (int)transform.parent.localPosition.y - (int)berry.transform.localPosition.y));
                //         }
                //     }


                // berryPoints.Add(new Vector3(0, 0, MathF.Abs((int)transform.position.x - (int)currentCollider.transform.localPosition.x)));


                // berryPoints.Add(new Vector3(0, 0, MathF.Abs((int)transform.position.x - (int)currentCollider.transform.localPosition.x)));

                //---
                // berryPoints.Add(new Vector3(transform.localPosition.x, 0, MathF.Abs((int)transform.position.x - currentCollider.transform.localPosition.x)));
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit && berryCounter < CellGeneration.Instance.GetHeight() + 2)
        {
            JustCheckCollision(startPoint + direction, direction);
        }


        // return new Vector3(0, 0, berryCounter);
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