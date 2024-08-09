using DG.Tweening;
using UnityEngine;

public class BerryCollisionHandler
{
    private readonly Berry berry;

    public BerryCollisionHandler(Berry berry)
    {
        this.berry = berry;
    }

    public void HandleCollision(Collision other)
    {
        if (other.gameObject.CompareTag("Frog"))
        {
            Debug.Log($"other's name {other.gameObject.name}");
            HandleFrogCollision(other.collider);
        }
        if (!other.gameObject.CompareTag("Berry")) return;

        Berry otherBerry = other.gameObject.GetComponent<Berry>();

        if (berry.GetLineRenderer() != null && otherBerry.GetLineRenderer() == null)
        {
            otherBerry.SetLineRenderer(berry.GetLineRenderer(), berry.TimeLeft);
        }
    }

    public void HandleTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Tongue"))
        {
            HandleTongueCollision(other);
        }
        else if (other.gameObject.CompareTag("Arrow"))
        {
            HandleArrowCollision(other);
        }
    }

    private void HandleFrogCollision(Collider other)
    {
        Debug.Log("Frog hit");

        berry.GetBoxCollider().isTrigger = true;
        var detectedBerries = other.GetComponent<Frog>().lineManager.GetDetectedBerries();

        berry.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.Linear).onComplete += () =>
        {
            detectedBerries.Remove(berry);
            Object.Destroy(berry.gameObject);
            if (detectedBerries.Count == 0)
            {
                // Object.Destroy(other.gameObject.transform.parent.gameObject);
                Object.Destroy(other.gameObject);
            }
        };
    }

    private void HandleTongueCollision(Collider other)
    {
        if (berry.IsTongueHit) return;

        berry.OnClickedOverWithTargetScale(new Vector3(1.5f, 1.5f, 1.5f));
        berry.SetTongueHit();

        var frog = other.transform.parent.parent.GetComponent<Frog>();
        frog.lineManager.GetDetectedBerries().Add(berry);
    }

    private void HandleArrowCollision(Collider other)
    {
        other.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.Linear).onComplete += () => Object.Destroy(other.gameObject);
    }
}