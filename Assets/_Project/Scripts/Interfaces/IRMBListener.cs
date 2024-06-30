using UnityEngine;

/// <summary>
/// Right mouse button clicked event listeners
/// </summary>
public abstract class IRMBListener : MonoBehaviour
{
    protected bool isSubscribed;
    protected abstract void ListenRMBEvent();
    protected abstract void StopListeningRMBEvent();

    protected abstract void OnDestroy();
}