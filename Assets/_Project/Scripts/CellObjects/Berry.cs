using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Berry : Clickable
{
    [SerializeField] private AudioClip normalStateClip;

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private MeshRenderer meshRenderer;

    private LineRenderer lineRenderer;

    private bool isHitable;
    private bool isTongueHit;
    private bool isDetected;
    private bool isLastBerryForFrog;
    private bool isLerping;

    private float lerpDuration;
    private float timeLeft;

    private void Update()
    {
        if (lineRenderer != null)
        {
            if (lineRenderer.positionCount < 2)
            {
                return;
            }

            if (!isLerping)
            {
                StartCoroutine(LerpPosition(transform.localPosition, lineRenderer.GetPosition(lineRenderer.positionCount - 2), lerpDuration));
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Berry"))
        {
            Berry otherBerry = other.gameObject.GetComponent<Berry>();

            if (lineRenderer != null && otherBerry.GetLineRenderer() == null)
            {
                otherBerry.SetLineRenderer(lineRenderer, timeLeft);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frog"))
        {
            boxCollider.isTrigger = true;
            List<Berry> detectedBerries = other.GetComponent<Frog>().GetDetectedObjects();

            transform.SetParent(null);

            transform.DOScale(new Vector3(0, 0, 0), .5f).SetEase(Ease.Linear).onComplete += () =>
            {
                Destroy(gameObject);

                detectedBerries.Remove(this);
                if (detectedBerries.Count == 0)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                    Destroy(other.gameObject);
                }
            };
        }

        if (other.CompareTag("Tongue"))
        {
            if (!isTongueHit)
            {
                OnClickedOverWithTargetScale(new Vector3(2, 2, 2));
                SetTongueHit();
                var frog = other.transform.parent.GetComponent<Frog>();
                frog.FreeBerriesForFrog += SetAsHitable;
                frog.detectedBerries.Add(this);
                isTongueHit = true;
            }
        }

        if (other.CompareTag("Arrow"))
        {
            // Create a general method for destroying / tweening objects out

            other.transform.DOScale(new Vector3(0, 0, 0), .5f).SetEase(Ease.Linear).onComplete += () => { Destroy(other.gameObject); };
        }
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
        if (!isTongueHit || isHitable)
        {
            base.OnClickedOverWithTargetScale(targetScale);

            AudioManager.Instance.PlayAudioClip(normalStateClip);
        }
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
        isHitable = false;
    }

    public void SetAsLastBerryForFrog()
    {
        isLastBerryForFrog = true;
    }

    public void SetAsDetected()
    {
        isDetected = true;
    }

    public void SetAsHitable()
    {
        isHitable = true;
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
}