using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PanelObjectControl : MonoBehaviour
{
    public static event EventHandler OnTimeToHideObject;

    [SerializeField] private Image image;

    private bool isCheckingPanel;
    private bool didCursorLeaveThePanel;

    private void Start()
    {
        AdjustTypeButton.OnObjectSelected += AdjustTypeButton_OnObjectSelected;
    }

    private void AdjustTypeButton_OnObjectSelected(object sender, EventArgs e)
    {
        isCheckingPanel = true;
    }

    private void Update()
    {
        if (!isCheckingPanel)
        {
            return;
        }

        if (didCursorLeaveThePanel)
        {
            CheckPanelForCursorToDeleteObject();
        }
        else
        {
            CheckCursorOnPanel();
        }
    }
    public void CheckCursorOnPanel()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            didCursorLeaveThePanel = true;
        }
    }

    public void CheckPanelForCursorToDeleteObject()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            OnTimeToHideObject?.Invoke(this, EventArgs.Empty);
            ResetValuesBackToNormal();
        }
    }

    private void ResetValuesBackToNormal()
    {
        isCheckingPanel = false;
        didCursorLeaveThePanel = false;
    }

}
