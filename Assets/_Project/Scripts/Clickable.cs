using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    private bool _isTweenable = true;

    [SerializeField] protected ProperNaming properNaming = new();

    public virtual void OnClickedOver()
    {
        ScaleUpAndDown();
    }

    private void ScaleUpAndDown()
    {
        if (_isTweenable)
        {
            _isTweenable = false;
            transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.12f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).onComplete += () => { _isTweenable = true; };
        }
    }

    public void ResetTweenState()
    {
        _isTweenable = false;
    }
}