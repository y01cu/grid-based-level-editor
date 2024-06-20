using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour, ISubscriber
{
    public static event Action OnRightMouseButtonClicked;

    public static event Action OnMoveUsed;

    public static event Action OnClearEvents;

    [SerializeField] private PlayerInput playerInput;

    private bool isClicked;

    private void Start()
    {
        playerInput.onActionTriggered += context => RightClickInteract(context);

        SubscribeToEvents();
    }

    private void RightClickInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRightMouseButtonClicked?.Invoke();
        }
    }

    // Subscribe

    private void DetectObjectUnderMouse()
    {
        Debug.Log("started detecting obj under the mouse");


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Arrow"))
            {
                Debug.Log("Arrow isn't clickable");
            }

            if (hitObject.CompareTag("Berry"))
            {
                hitObject.GetComponent<Berry>().OnClickedOver();
            }

            if (hitObject.CompareTag("Frog"))
            {
                Debug.Log("hit frog");
                hitObject.GetComponent<Frog>().OnClickedOver();
                OnMoveUsed?.Invoke();
            }
        }
    }

    public void SubscribeToEvents()
    {
        OnRightMouseButtonClicked += DetectObjectUnderMouse;
        OnClearEvents += UnsubscribeFromEvents;
    }

    public void UnsubscribeFromEvents()
    {
        OnRightMouseButtonClicked -= DetectObjectUnderMouse;
        OnClearEvents -= UnsubscribeFromEvents;
    }

    public static void TriggerClearingEvents()
    {
        OnClearEvents?.Invoke();
    }
}