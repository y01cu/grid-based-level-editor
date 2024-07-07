using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class Clickable : CellObject
{
    protected bool isTweenable;
    
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