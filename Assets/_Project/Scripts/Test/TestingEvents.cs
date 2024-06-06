using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TestingEvents : MonoBehaviour
{
    public static event Action OnRightMouseButtonClicked;

    [SerializeField] private PlayerInput playerInput;

    private void Start()
    {
        playerInput.onActionTriggered += context => LogMessage(context);
    }

    public void LogMessage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRightMouseButtonClicked?.Invoke();
        }
    }
}