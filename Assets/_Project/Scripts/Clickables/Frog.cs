using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using QFSW.QC;
using UnityEngine;


public class Frog : Clickable
{
    public event Action FreeBerriesForFrog;
    public event Action MoveBerriesToFrog;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private LayerMask collisionMask;

    private bool _isTongueOutside;

    [SerializeField] private TongueNew newTongue;
    

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
    }

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
        
        // newTongue.OnMouseRightClick();
        
        // ---

        Vector3 startPoint = transform.TransformPoint(lineRenderer.GetPosition(0));
        Vector3 endPoint = new Vector3(0, 0, 3);
        
        TweenBetweenTwoPoints(startPoint);
    }

    private bool _isTongueReachedEnd;

    private float _tweenDuration = 1.2f;
    private float _interval = 0.15f;

    private List<GameObject> _detectedObjects = new();

    public List<GameObject> GetDetectedObjects()
    {
        return _detectedObjects;
    }

    // This function below can be made async

    private bool _isReachedEnd;

    private void TweenBetweenTwoPoints(Vector3 startPoint)
    {
        _isTongueOutside = true;

        Vector3 worldPosition = Vector3.zero;

        RaycastHit[] hits = null;

        Sequence mySequence = DOTween.Sequence();

        // Ray direction change condition must be considered here.
        // JustCheckCollision(startPoint, transform.TransformPoint(endPoint));
        
        // Vector3 frogRotation = transform.parent.localRotation.eulerAngles 
        //
        // if (frogRotation.)
        // {
        //     
        // }

        Vector3 rotation = transform.parent.localRotation.eulerAngles; 

        // bool isUp = rotation.x == -90;
        // bool isDown = rotation.x == 90;

        Vector3 direction = rotation.x switch
        {
            90 => Vector3.down,
            270 => Vector3.up,
        };

        Vector3 endPoint = JustCheckCollision(startPoint, direction);

        int lastPositionIndex = lineRenderer.positionCount - 1;

        mySequence.Append(
                DOTween.To(() => lineRenderer.GetPosition(lastPositionIndex), x =>
                {
                    worldPosition = transform.TransformPoint(x);
                    lineRenderer.SetPosition(lastPositionIndex, x);
                    hits = CheckCollision(startPoint, worldPosition);
                }, endPoint, _tweenDuration)
            )
            .AppendInterval(_interval).AppendCallback(() =>
            {
                Debug.Log("reached end");
                // for (int i = 0; i < hits.Length; i++)
                // {
                //     hits[i].collider.isTrigger = false;
                // }

                _detectedObjects[^1].GetComponent<Berry>().SetLineRenderer(lineRenderer);
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

    private int collisionCounter = 0;

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
                // Debug.Log("arrow hit");

                isAnyArrowHit = true;

                Arrow.Direction arrowDirection = currentCollider.GetComponent<Arrow>().direction;

                switch (arrowDirection)
                {
                    case Arrow.Direction.Left:
                        Debug.Log("la");
                        JustCheckCollision(currentCollider.transform.position, -Vector3.right);
                        break;
                    case Arrow.Direction.Right:
                        Debug.Log("ra");
                        break;
                    case Arrow.Direction.Up:
                        Debug.Log("ua");
                        break;
                    case Arrow.Direction.Down:
                        Debug.Log("da");
                        break;

                    default:
                        Debug.Log("la");
                        break;
                }
            }

            if (currentCollider.CompareTag("Berry"))
            {
                Debug.Log("berry collision");
                collisionCounter++;
                Berry berry = currentCollider.GetComponent<Berry>();
                if (berry.isLastForFrog)
                {
                    // Stop tongue movement
                }
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit && collisionCounter < CellGeneration.Instance.GetHeight())
        {
            JustCheckCollision(startPoint + direction, direction);
        }

        else
        {
            Debug.Log("total detected obj count: " + collisionCounter);
        }

        return new Vector3(0, 0, collisionCounter);
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
            if (hits[i].collider.CompareTag("Arrow"))
            {
                Vector3 startPosition = hits[i].collider.transform.position;
                lineRenderer.positionCount++;
                // lineRenderer.SetPosition(lineRenderer.positionCount-1, );
                
                return CheckCollision(startPosition, new Vector3(startPosition.x -1, startPosition.y, startPosition.z));
                
            }
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