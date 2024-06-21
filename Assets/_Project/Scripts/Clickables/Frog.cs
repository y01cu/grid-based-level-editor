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

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LayerMask collisionMask;

    private bool _isTongueOutside;

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

    public override void OnClickedOverWithTargetScale(Vector3 targetScale)
    {
        if (_isTongueOutside)
        {
            return;
        }

        base.OnClickedOverWithTargetScale(targetScale);

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

        if (detectedObjects.Count > 0)
        {
            if (!isObstacleHit)
            {
                points.Add(transform.parent.InverseTransformPoint(detectedObjects[^1].transform.localPosition));
            }
        }

        time = detectedObjects.Count * 2.5f;
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

            sequence.Append(DOTween.To(() => start, x =>
            {
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

        _isTongueOutside = false;

        isObstacleHit = false;
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

    private List<GameObject> detectedObjects = new();
    private float segmentDuration;

    private void JustCheckCollision(Vector3 startPoint, Vector3 direction)
    {
        // Vector3 direction = endPoint - startPoint;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        if (hits.Length > 0)
        {
            var firstHit = hits[0].transform;
            // bool isSameColor = firstHit.name[0] == gameObject.name[0];
            // bool isCollidedWithFrog = firstHit.name[2] == 'F';

            if (firstHit.name[0] != gameObject.name[0])
            {
                // is obstacle

                Debug.Log("first letters are not same", hits[0].collider);

                firstHit.GetComponent<CellObject>().SetAsObstacle();

                Vector3 obstaclePoint;
                if (firstHit.transform.parent != null)
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.parent.transform.localPosition);
                }
                else
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.localPosition);
                }

                detectedObjects.Add(firstHit.gameObject);
                points.Add(obstaclePoint);
                Debug.Log("added obstacle point: " + obstaclePoint);
                isObstacleHit = true;
                return;
            }

            if (firstHit.name[2] == 'F')
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
                Berry berry = currentCollider.GetComponent<Berry>();

                if (berry.gameObject.name[0] == gameObject.name[0] && !berry.IsDetected())
                {
                    berry.SetAsDetected();

                    detectedObjects.Add(berry.gameObject);
                }

                if (berry.IsLastBerryForFrog())
                {
                    return;
                }
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit)
        {
            JustCheckCollision(startPoint + direction, direction);
        }
    }

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;


    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayAudioClip(obstacleStateClip);

        skinnedMeshRenderer.material = obstacleMaterial;

        await Task.Delay(1000);

        skinnedMeshRenderer.material = normalMaterial;

        CleanObstacleState();
    }
}