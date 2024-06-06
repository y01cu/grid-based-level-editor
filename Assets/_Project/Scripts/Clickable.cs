using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    private bool _isTweening;
    public void OnClickedOver()
    {
        if (!_isTweening)
        {
            _isTweening = true;
            transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).onComplete += () =>
            {
                _isTweening = false;
            };
        }
    }
}