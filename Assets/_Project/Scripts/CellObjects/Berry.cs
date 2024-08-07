using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Berry : Clickable
{
    [SerializeField] private AudioClip normalStateClip;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshRenderer meshRenderer;

    private LineRenderer lineRenderer;
    private bool isTongueHit;
    private bool isDetected;
    private bool isLastBerryForFrog;
    private bool isLerping;
    private float lerpDuration;
    private float timeLeft;

    private BerryCollisionHandler collisionHandler;

    public bool IsTongueHit => isTongueHit;
    public float TimeLeft => timeLeft;

    private void Awake()
    {
        collisionHandler = new BerryCollisionHandler(this);
    }

    private void Start()
    {
        isTweenable = true;
    }

    private void Update()
    {
        if (lineRenderer == null || lineRenderer.positionCount < 2 || isLerping) return;

        StartCoroutine(LerpPosition(transform.localPosition, lineRenderer.GetPosition(lineRenderer.positionCount - 2), lerpDuration));
    }

    private void OnCollisionEnter(Collision other)
    {
        collisionHandler.HandleCollision(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionHandler.HandleTrigger(other);
    }

    private IEnumerator LerpPosition(Vector3 start, Vector3 end, float duration)
    {
        isLerping = true;
        float time = 0;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            timeLeft = duration - time;
            yield return null;
        }

        transform.localPosition = end;
        isLerping = false;
    }

    public override void OnClickedOverWithTargetScale(Vector3 targetScale)
    {
        if (IsTongueHit) return;

        base.OnClickedOverWithTargetScale(targetScale);
        AudioManager.Instance.PlayAudioClip(normalStateClip);
    }

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayAudioClip(obstacleStateClip);
        meshRenderer.material = obstacleMaterial;
        await Task.Delay(1000);
        meshRenderer.material = normalMaterial;
    }

    public void TurnBackToNormalState()
    {
        isDetected = false;
        isTongueHit = false;
    }

    public void SetLineRenderer(LineRenderer newLineRenderer, float newDuration)
    {
        lerpDuration = newDuration;
        lineRenderer = newLineRenderer;
        transform.SetParent(lineRenderer.transform);
    }

    public void SetTargetBoxCollider(BoxCollider givenTargetBoxCollider)
    {
        transform.SetParent(givenTargetBoxCollider.transform);
    }

    public void SetTongueHit()
    {
        isTongueHit = true;
    }

    public void SetAsLastBerryForFrog()
    {
        isLastBerryForFrog = true;
    }

    public void SetAsDetected()
    {
        isDetected = true;
    }

    public bool IsLastBerryForFrog()
    {
        return isLastBerryForFrog;
    }

    public bool IsDetected()
    {
        return isDetected;
    }

    public LineRenderer GetLineRenderer()
    {
        return lineRenderer;
    }

    // Expose necessary internal state for collision handling.
    public BoxCollider GetBoxCollider() => boxCollider;

    public override void AdjustTransformForSetup()
    {
        //transform.Translate(0, 0.5f, 0);
        //// ---
        //var additionalRotation = new Vector3(180, 90, 0);
        //Debug.Log($"rotation adjusted for berry {additionalRotation}");
        //transform.Rotate(additionalRotation);
        // ---
    }

    public override void RotateByAngle(Vector3 angle)
    {
        // Berry won't rotate in the level editor
    }
}