using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private List<Vector3> points = new();

    public List<GameObject> detectedObjects = new();

    public List<Berry> detectedBerries = new();

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private LayerMask collisionMask;

    private bool isLineOutside;
    private bool isObstacleHit;

    private void Start()
    {
        SetUpLineRenderer();
        points.Add(lineRenderer.GetPosition(0));
    }

    private void SetUpLineRenderer()
    {
        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;
        lineRenderer.positionCount = 1;
    }

    public void StartProcess(Vector3 startPoint, Vector3 direction, CellGeneration.OrderType orderType, CellBase.ObjectColor objectColor)
    {
        JustCheckCollision(startPoint, direction, orderType, objectColor);
        // JustCheckCollision(startPoint, direction);

        if (detectedObjects.Count > 0)
        {
            if (!isObstacleHit)
            {
                points.Add(transform.parent.InverseTransformPoint(detectedObjects[^1].transform.localPosition));
            }
        }

        Debug.Log(detectedBerries.Count);

        // time = detectedObjects.Count * 3.5f;
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

        float segmentDuration = totalDuration / points.Count;

        // segmentDuration = totalDuration / points.Count;

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

            sequence.Append(DOTween.To(() => start, x =>
            {
                Debug.Log("lolololo");
                UpdateLine(index, x);
                if (boxCollider != null)
                {
                    boxCollider.center = x;
                }
            }, end, segmentDuration).SetEase(Ease.Linear));
        }

        sequence.AppendCallback(() =>
        {
            if (!isObstacleHit && detectedBerries[^1].IsLastBerryForFrog())
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
            else
            {
                var lastDetectedObject = detectedObjects[^1];
                lastDetectedObject.GetComponent<CellObject>().HandleBeingObstacle();
                Debug.Log("this is the obstacle obj", lastDetectedObject);
            }
        });


        // Animate backward
        for (int i = points.Count - 1; i > 0; i--)
        {
            int index = i;
            Vector3 end = points[index - 1];
            Vector3 start = points[index];

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

            var newPosition = position;

            lineRenderer.SetPosition(index, newPosition);
        }
    }


    private void ResetBackToInitialState()
    {
        for (int i = 0; i < detectedBerries.Count; i++)
        {
            detectedBerries[i].TurnBackToNormalState();
        }

        detectedBerries.Clear();

        detectedObjects.Clear();

        points.Clear();

        lineRenderer.positionCount = 1;

        lineRenderer.SetPosition(0, transform.localPosition);

        points.Add(lineRenderer.GetPosition(0));

        isLineOutside = false;

        isObstacleHit = false;

        Debug.Log("reset back to init state");
    }

    public void JustCheckCollision(Vector3 startPoint, Vector3 direction, CellGeneration.OrderType orderType, CellBase.ObjectColor objectColor)
    {
        // TODO: Handle positions in a better way


        Debug.DrawRay(startPoint, direction, Color.black, 2f);

        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        Debug.Log("hit count: " + hits.Length, this);

        var firstHit = hits[0].transform;
        var firstCellObject = hits[0].transform.GetComponent<CellObject>();

        if (hits.Length > 0)
        {
            bool isDifferentColor = firstHit.transform.GetComponent<CellObject>().objectColor != objectColor;


            Debug.Log("is dif color: " + isDifferentColor);
            if (isDifferentColor)
            {
                // is obstacle

                Vector3 obstaclePoint;

                if (firstHit.transform.parent != null)
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.parent.transform.localPosition);
                }
                else
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.localPosition);
                }

                // Debug.Log("btw added point: " + obstaclePoint);

                detectedObjects.Add(firstHit.gameObject);
                points.Add(obstaclePoint);
                isObstacleHit = true;
                return;
            }

            if (firstHit.transform.GetComponent<CellObject>().objectType == CellBase.ObjectType.Frog)
            {
                return;
            }
        }

        bool isAnyArrowHit = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider currentCollider = hits[i].collider;

            if (currentCollider.CompareTag("Arrow"))
            {
                isAnyArrowHit = true;

                Direction arrowDirection = currentCollider.GetComponent<Arrow>().GetDirection();

                var newPoint = Vector3.zero;

                switch (arrowDirection)
                {
                    case Direction.Left:

                        if (orderType == CellGeneration.OrderType.Column)
                        {
                            // 0-> up
                            // 90-> right

                            switch ((int)transform.parent.localRotation.eulerAngles.y)
                            {
                                case 0:
                                    // up
                                    Debug.Log("upp--: " + (currentCollider.transform.position.x - transform.parent.position.x));
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("new point of it: " + newPoint);

                                    break;
                                case 90:
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("rightt");
                                    //right
                                    break;
                                case 180:
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("down");
                                    // down
                                    break;
                                case 270:
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));


                                    Debug.Log("left");
                                    // left
                                    break;
                            }
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
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));

                                    break;
                                case 90:
                                    newPoint = new Vector3((int)transform.parent.position.y - (int)currentCollider.transform.position.y, 0, MathF.Abs((int)transform.position.x - (int)currentCollider.transform.localPosition.x));
                                    //right
                                    break;
                                case 180:
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));
                                    // down
                                    break;
                                case 270:
                                    newPoint = new Vector3(MathF.Abs(transform.parent.position.x - currentCollider.transform.position.x), 0, MathF.Abs((int)transform.position.y - (int)currentCollider.transform.localPosition.y));
                                    // left
                                    break;
                            }
                        }

                        points.Add(newPoint);
                        Debug.Log("btw added point: " + newPoint);

                        JustCheckCollision(currentCollider.transform.position, Vector3.left, orderType, objectColor);

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
                        Debug.Log("btw added point: " + newPoint);

                        JustCheckCollision(currentCollider.transform.position, Vector3.right, orderType, objectColor);

                        break;
                    case Direction.Up:
                        newPoint = new Vector3((int)currentCollider.transform.localPosition.y - (int)transform.parent.position.y, 0, MathF.Abs(currentCollider.transform.position.x - transform.parent.position.x));
                        points.Add(newPoint);
                        Debug.Log("btw added point: " + newPoint);

                        JustCheckCollision(currentCollider.transform.position, Vector3.up, orderType, objectColor);

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
                        Debug.Log("btw added point: " + newPoint);

                        JustCheckCollision(currentCollider.transform.position, Vector3.down, orderType, objectColor);

                        break;
                }
            }

            if (currentCollider.CompareTag("Berry"))
            {
                Debug.Log("hit berry");

                Berry berry = currentCollider.GetComponent<Berry>();

                bool isSameColor = berry.objectColor == objectColor;

                if (isSameColor && !berry.IsDetected())
                {
                    berry.SetAsDetected();

                    detectedObjects.Add(berry.gameObject);
                    Debug.Log("added berry");
                    Debug.Log("detected obj count: " + detectedObjects.Count);
                }

                if (berry.IsLastBerryForFrog())
                {
                    Debug.Log("found last berry");
                    return;
                }
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit)
        {
            // Debug.Log("started");
            JustCheckCollision(startPoint + direction, direction, orderType, objectColor);
        }
    }

    public bool IsLineOutside()
    {
        return isLineOutside;
    }
}