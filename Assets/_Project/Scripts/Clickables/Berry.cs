using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Berry : Clickable
{
    private bool _isHitable;
    private bool _isTongueHit;

    private Vector3 _forceDirection;
    [SerializeField] private Rigidbody rigidbody;

    [HideInInspector] public bool isMoving;

    private LineRenderer _lineRenderer;

    private float _interpolationFactor; //

    [SerializeField] private BoxCollider boxCollider;

    private Vector3 lineRendererVector3;

    public bool isLastForFrog;

    private void Start()
    {
        _isLineRendererNotNull = _lineRenderer != null;
        gameObject.name = properNaming.GetProperName();
    }

    private void Update()
    {
        if (_lineRenderer != null)
        {
            if (_lineRenderer.positionCount < 2)
            {
                return;
            }

            var position = transform.position;
            Vector3 currentPosition = position;

            Vector3 targetPosition = _lineRenderer.transform.parent.position;
            // Vector3 targetPosition = _lineRenderer.GetPosition(0);

            _interpolationFactor += Time.deltaTime / Vector3.Distance(currentPosition, targetPosition);
            position = Vector3.Lerp(currentPosition, new Vector3(targetPosition.x, targetPosition.y, position.z), _interpolationFactor * 0.02f);
            transform.position = position;
        }
    }

    public void SetLineRenderer(LineRenderer newLineRenderer)
    {
        _lineRenderer = newLineRenderer;
        isMoving = true;
    }

    public LineRenderer GetLineRenderer()
    {
        return _lineRenderer;
    }

    public override void OnClickedOver()
    {
        if (!_isTongueHit || _isHitable)
        {
            base.OnClickedOver();
        }
    }

    public void SetTongueHit()
    {
        _isTongueHit = true;
        _isHitable = false;
    }

    public void SetAsHitable()
    {
        _isHitable = true;
    }

    public void SetDirection(Vector3 forceDirection)
    {
        _forceDirection = forceDirection;
    }

    public void MoveToFrog()
    {
        rigidbody.AddForce(_forceDirection);
    }

    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Berry"))
        {
            Berry otherBerry = other.gameObject.GetComponent<Berry>();

            if (isMoving && !otherBerry.isMoving)
            {
                Debug.Log("collided with a stopped berry");
            }

            if (_lineRenderer != null && otherBerry.GetLineRenderer() == null)
            {
                otherBerry.SetLineRenderer(_lineRenderer);
            }
        }
    }

    private WaitForSeconds frogDestroyDelay = new(0.6f);
    private bool _isLineRendererNotNull;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frog") && isMoving)
        {
            boxCollider.isTrigger = true;
            List<GameObject> detectedObjects = other.GetComponent<Frog>().GetDetectedObjects();

            // for (int i = 0; i < detectedObjects.Count; i++)
            // {
            // }

            UnityEngine.Debug.Log("name of the other one: " + other.gameObject.name);

            transform.DOScale(new Vector3(0, 0, 0), .15f).SetEase(Ease.Linear).onComplete += () =>
            {
                Destroy(gameObject);
                Debug.Log("destroyed here");

                detectedObjects.Remove(gameObject);
                if (detectedObjects.Count == 0)
                {
                    Destroy(other.gameObject);
                    Debug.Log("destroyed there");
                }
            };

            // yield return new WaitUntil(() => detectedObjects.Count == 0);
            // Destroy(other.gameObject);
            // Debug.Log("game object's destroyed");
        }
    }
}