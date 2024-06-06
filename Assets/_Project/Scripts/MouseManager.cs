using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private bool isClicked;

    private void Start()
    {
        Subscribe();
    }


    private void RightClickInteract()
    {
        DetectObjectUnderMouse();
        // Debug.Log("right clicked on me", this);
    }

    // Subscribe

    private void Subscribe()
    {
        TestingEvents.OnRightMouseButtonClicked += RightClickInteract;
    }

    private void UnSubscribe()
    {
        TestingEvents.OnRightMouseButtonClicked -= RightClickInteract;
    }


    private void DetectObjectUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Arrow"))
            {
                hitObject.GetComponent<Clickable>().OnClickedOver();
            }

            if (hitObject.CompareTag("Berry"))
            {
                hitObject.GetComponent<Clickable>().OnClickedOver();
            }

            if (hitObject.CompareTag("Frog"))
            {
                hitObject.GetComponent<Clickable>().OnClickedOver();
            }
        }
    }
}