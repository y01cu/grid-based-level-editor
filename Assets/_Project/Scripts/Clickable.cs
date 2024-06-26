using DG.Tweening;
using UnityEngine;

public abstract class Clickable : CellObject
{
    private bool isTweenable = true;

    // [SerializeField] protected ProperNaming properNaming = new();

    public virtual void OnClickedOverWithTargetScale(Vector3 targetScale)
    {
        ScaleUpAndDown(targetScale);
    }

    protected void ScaleUpAndDown(Vector3 targetScale)
    {
        if (isTweenable)
        {
            isTweenable = false;
            transform.DOScale(targetScale, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).onComplete += ResetAsTweenable;
        }
    }

    public void ResetAsTweenable()
    {
        isTweenable = true;
    }
}