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

            transform.localPosition = Vector3.Lerp(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), .9f * Time.deltaTime); //  0.02f

            // Debug.Log(targetBoxCollider + " target box collider");

            // Vector3 targetPosition = transform.TransformPoint(targetBoxCollider.center);
            // Vector3 targetPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);


            // if (!isLerping)
            // {
            //     StartCoroutine(LerpPosition());
            // }


            
// -----

            // Vector3 targetPosition = new Vector3(transform.position.x - _lineRenderer.GetPosition(_lineRenderer.positionCount - 1).x, transform.position.y - (transform.position.y - _lineRenderer.GetPosition(_lineRenderer.positionCount - 1).z), transform.position.z);
            //
            //
            // _interpolationFactor = Time.deltaTime / (Vector3.Distance(currentPosition, targetPosition) * 3f);
            //
            // var newTargetPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 2);
            // //
            // currentPosition = Vector3.Lerp(currentPosition, newTargetPosition, .001f); //  0.02f
            //
            // Debug.Log("tPos: " + transform.position + " | tLocPos: " + transform.localPosition + " | tar: " + newTargetPosition);
            // transform.position = currentPosition;

            // -----

            // Vector3 targetPosition = _lineRenderer.transform.parent.position;

            // Vector3 targetPosition = new Vector3(transform.position.x - _lineRenderer.transform.parent.parent.transform.position.x, transform.position.y, transform.position.z);
            // Debug.Log("new target pos: " + newTargetPosition);

            // currentPosition = Vector3.Lerp(currentPosition, new Vector3(targetPosition.x, targetPosition.y, currentPosition.z), _interpolationFactor); //  0.02f
        }
    }

    private float lerpDuration = 3f;

    private IEnumerator LerpPosition()
    {
        isLerping = true;
        float timeElapsed = 0f;

        // Vector3 targetPosition = Vector3.zero;
        while (timeElapsed < lerpDuration)
        {
            Debug.Log("lerping btw");
            timeElapsed += Time.deltaTime;

            var distance = transform.localPosition - _lineRenderer.GetPosition(_lineRenderer.positionCount - 2);
            transform.localPosition = Vector3.Lerp(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), timeElapsed / lerpDuration*10); //  0.02f


            // transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y, transform.position.z), 3.5f / 20 * Time.deltaTime); //  0.02f
            // transform.localPosition = Vector3.Lerp(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2),   .005f*(timeElapsed / lerpDuration)); //  0.02f
            yield return null;
        }

        // transform.position = targetPosition;
        isLerping = false;

        // ---

        // isLerping = true;
        // float timeElapsed = 0f;
        //
        // Vector3 targetPosition = Vector3.zero;
        // while (timeElapsed < lerpDuration)
        // {
        //     // targetPosition = new Vector3(transform.position.x - _lineRenderer.GetPosition(_lineRenderer.positionCount - 1).x, transform.position.y - (transform.position.y - _lineRenderer.GetPosition(_lineRenderer.positionCount - 1).z), transform.position.z);
        //     timeElapsed += Time.deltaTime;
        //     // transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y, transform.position.z), 3.5f / 20 * Time.deltaTime); //  0.02f
        //     // transform.position = Vector3.Lerp(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), 3.5f / 20 * Time.deltaTime); //  0.02f
        //     transform.position = Vector3.Lerp(transform.localPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), timeElapsed); //  0.02f
        //     yield return null;
        // }
        //
        // transform.position = targetPosition;
        // isLerping = false;
    }

    public void SetLineRenderer(LineRenderer newLineRenderer)
    {
        _lineRenderer = newLineRenderer;
        transform.SetParent(_lineRenderer.transform);
        Debug.Log("line renderer is set", this);
        isMoving = true;

        var currentPosition = transform.localPosition;

        var targetPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 2);
        Debug.Log("current pos: " + currentPosition + " | target pos: " + targetPosition);
        Vector3.Lerp(transform.localPosition, targetPosition, 1);
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