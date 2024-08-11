using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : IRMBListener
{
    public static event Action OnRightMouseButtonClicked;
    public static event Action OnMoveUsed;

    [SerializeField] private PlayerInput playerInput;

    private InputAction scrollAction;
    private InputAction rmbClickAction;

    [SerializeField] private Texture2D cursorTexture2D;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture2D, Vector2.zero, CursorMode.Auto);

        // playerInput.onActionTriggered += context => RightClickInteract(context);

        if (!isSubscribed)
        {
            ListenRMBEvent();
        }

        rmbClickAction = playerInput.actions["Mouse Button"];
        scrollAction = playerInput.actions["Mouse Scroll"];

        if (rmbClickAction != null)
        {
            rmbClickAction.performed += RightClickInteract;
        }
        else
        {
            Debug.LogError("rmb click action not found!");
        }

        if (scrollAction != null)
        {
            scrollAction.performed += HandleScrollAction;
        }
        else
        {
            Debug.LogError("scroll action not found!");
        }
    }

    private void HandleScrollAction(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();
        if (scrollValue.y >= 0)
        {
            ObjectGhost.Instance.RotateCurrentObjectWithAngle(new Vector3(0, 90, 0));
        }
        else if (scrollValue.y < 0)
        {
            ObjectGhost.Instance.RotateCurrentObjectWithAngle(new Vector3(0, -90, 0));
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
                hitObject.GetComponent<Berry>().OnClickedOverWithTargetScale(new Vector3(1.5f, 1.5f, 1.5f));
            }

            if (hitObject.CompareTag("Frog"))
            {
                Frog frog = hitObject.GetComponent<Frog>();

                if (!frog.lineManager.IsLineOutside)
                {
                    OnMoveUsed?.Invoke();
                }

                frog.OnClickedOverWithTargetScale(new Vector3(1.4f, 1.4f, 1.4f));
            }
        }
    }

    protected override void ListenRMBEvent()
    {
        OnRightMouseButtonClicked += DetectObjectUnderMouse;

        isSubscribed = true;
    }

    protected override void StopListeningRMBEvent()
    {
        OnRightMouseButtonClicked -= DetectObjectUnderMouse;

        isSubscribed = false;
    }

    protected override void OnDestroy()
    {
        StopListeningRMBEvent();
    }
}