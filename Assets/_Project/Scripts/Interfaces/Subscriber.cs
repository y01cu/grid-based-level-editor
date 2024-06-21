using UnityEngine;

public abstract class Subscriber : MonoBehaviour
{
    protected bool isSubscribed;
    protected abstract void SubscribeToEvents();
    protected abstract void UnsubscribeFromEvents();

    protected abstract void OnDestroy();
}