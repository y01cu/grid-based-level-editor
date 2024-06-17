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

    private float _interpolationFactor = 7.5f; //

    [SerializeField] private BoxCollider boxCollider;

    private Vector3 lineRendererVector3;


    public bool isLastForFrog;

    [SerializeField] private BoxCollider targetBoxCollider;

    public void SetTargetBoxCollider(BoxCollider givenTargetBoxCollider)
    {
        targetBoxCollider = givenTargetBoxCollider;
        transform.SetParent(givenTargetBoxCollider.transform);
    }

    private void Start()
    {
        _isLineRendererNotNull = _lineRenderer != null;
        gameObject.name = properNaming.GetProperName();
    }

    private bool isMovingHorizontally;

    private void Update()
    {
        if (_lineRenderer != null)
        {
            if (_lineRenderer.positionCount < 2)
            {
                return;
            }


            if (!isLerping)
            {
                StartCoroutine(LerpPosition(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), lerpDuration));
            }

            // transform.localPosition = Vector3.Lerp(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), .9f * Time.deltaTime); //  0.02f
        }
    }

    private float lerpDuration;

    private float timeLeft;


    private IEnumerator LerpPosition(Vector3 start, Vector3 end, float duration)
    {
        isLerping = true;


        float time = 0;
        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;

            Debug.Log("spent time: " + time / duration);

            timeLeft = duration - time;
            yield return null;
        }

        transform.localPosition = end;

        isLerping = false;
    }

    public void SetLineRenderer(LineRenderer newLineRenderer, float newDuration)
    {
        Debug.Log("berry started moving with time: " + newDuration, this);

        lerpDuration = newDuration;
        _lineRenderer = newLineRenderer;
        transform.SetParent(_lineRenderer.transform);
        Debug.Log("line renderer is set", this);
        isMoving = true;

        var currentPosition = transform.localPosition;

        var targetPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 2);
        Debug.Log("current pos: " + currentPosition + " | target pos: " + targetPosition);
        // Vector3.Lerp(transform.localPosition, targetPosition, 1);
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
                otherBerry.SetLineRenderer(_lineRenderer, timeLeft);
            }
        }
    }

    private WaitForSeconds frogDestroyDelay = new(0.6f);
    private bool _isLineRendererNotNull;
    private bool isLerping;
    private bool _lineRendererNotNull;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frog") && isMoving)
        {
            boxCollider.isTrigger = true;
            List<Berry> detectedBerries = other.GetComponent<Frog>().GetDetectedObjects();

            // for (int i = 0; i < detectedObjects.Count; i++)
            // {
            // }

            UnityEngine.Debug.Log("name of the other one: " + other.gameObject.name);

            transform.DOScale(new Vector3(0, 0, 0), .15f).SetEase(Ease.Linear).onComplete += () =>
            {
                Destroy(gameObject);
                Debug.Log("destroyed here");

                detectedBerries.Remove(this);
                if (detectedBerries.Count == 0)
                {
                    Destroy(other.gameObject);
                    Debug.Log("destroyed there");
                }
            };

            // yield return new WaitUntil(() => detectedObjects.Count == 0);
            // Destroy(other.gameObject);
            // Debug.Log("game object's destroyed");
        }

        if (other.CompareTag("Tongue"))
        {
            Debug.Log("Tongue entered.");

            if (!_isTongueHit)
            {
                OnClickedOver();
                SetTongueHit();
                var frog = other.transform.parent.GetComponent<Frog>();
                frog.FreeBerriesForFrog += SetAsHitable;
                frog.detectedBerries.Add(this);
                _isTongueHit = true;
            }
        }
    }
}