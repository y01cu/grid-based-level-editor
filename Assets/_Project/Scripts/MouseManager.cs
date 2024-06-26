using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : Subscriber
{
    public static event Action OnRightMouseButtonClicked;
    public static event Action OnMoveUsed;

    [SerializeField] private PlayerInput playerInput;

    private void Start()
    {
        playerInput.onActionTriggered += context => RightClickInteract(context);

        if (!isSubscribed)
        {
            SubscribeToEvents();
        }
    }

    private void RightClickInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRightMouseButtonClicked?.Invoke();
        }
    }

    private void DetectObjectUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Berry"))
            {
                hitObject.GetComponent<Berry>().OnClickedOverWithTargetScale(new Vector3(2, 2, 2));
            }

            if (hitObject.CompareTag("Frog"))
            {
                Frog frog = hitObject.GetComponent<Frog>();

                if (!frog.lineManager.IsLineOutside())
                {
                    OnMoveUsed?.Invoke();
                }

                frog.OnClickedOverWithTargetScale(new Vector3(1.4f, 1.4f, 1.4f));
            }
        }
    }

    protected override void SubscribeToEvents()
    {
        OnRightMouseButtonClicked += DetectObjectUnderMouse;

        isSubscribed = true;
    }

    protected override void UnsubscribeFromEvents()
    {
        OnRightMouseButtonClicked -= DetectObjectUnderMouse;

        isSubscribed = false;
    }

    protected override void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}