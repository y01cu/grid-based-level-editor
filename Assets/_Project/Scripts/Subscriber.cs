using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subscriber : MonoBehaviour
{
    protected bool isSubscribed;
    protected abstract void SubscribeToEvents();
    protected abstract void UnsubscribeFromEvents();

    protected abstract void OnDestroy();
}