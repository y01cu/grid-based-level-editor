using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class Clickable : CellObject
{
    private bool _isTweenable = true;

    [SerializeField] protected ProperNaming properNaming = new();

    public virtual void OnClickedOverWithTargetScale(Vector3 targetScale)
    {
        ScaleUpAndDown(targetScale);
    }

    protected void ScaleUpAndDown(Vector3 targetScale)
    {
        if (_isTweenable)
        {
            _isTweenable = false;
            transform.DOScale(targetScale, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).onComplete += () => { _isTweenable = true; };
        }
    }

    public void ResetTweenState()
    {
        _isTweenable = false;
    }
}