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
        if (!other.gameObject.CompareTag("Berry")) return;

        Berry otherBerry = other.gameObject.GetComponent<Berry>();

        if (berry.GetLineRenderer() != null && otherBerry.GetLineRenderer() == null)
        {
            otherBerry.SetLineRenderer(berry.GetLineRenderer(), berry.TimeLeft);
        }
    }

    public void HandleTrigger(Collider other)
    {
        switch (other.tag)
        {
            case "Frog":
                HandleFrogCollision(other);
                break;
            case "Tongue":
                HandleTongueCollision(other);
                break;
            case "Arrow":
                HandleArrowCollision(other);
                break;
        }
    }

    private void HandleFrogCollision(Collider other)
    {
        berry.GetBoxCollider().isTrigger = true;
        var detectedBerries = other.GetComponent<Frog>().lineManager.GetDetectedBerries();

        berry.transform.SetParent(null);
        berry.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.Linear).onComplete += () =>
        {
            Object.Destroy(berry.gameObject);
            detectedBerries.Remove(berry);
            if (detectedBerries.Count == 0)
            {
                Object.Destroy(other.gameObject.transform.parent.gameObject);
                Object.Destroy(other.gameObject);
            }
        };
    }

    private void HandleTongueCollision(Collider other)
    {
        if (berry.IsTongueHit) return;

        berry.OnClickedOverWithTargetScale(new Vector3(2, 2, 2));
        berry.SetTongueHit();

        var frog = other.transform.parent.GetComponent<Frog>();
        frog.lineManager.GetDetectedBerries().Add(berry);
    }

    private void HandleArrowCollision(Collider other)
    {
        other.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.Linear).onComplete += () => Object.Destroy(other.gameObject);
    }
}