using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestIfSeen : MonoBehaviour
{
    public Camera activeCamera;
    public GameObject targetObject;

    void Update()
    {
        if (IsTargetSeen())
        {
            Debug.Log("Target is seen by the XR Camera");
        }
        else
        {
            Debug.Log("Target is not seen by the XR Camera");
        }
    }

    bool IsTargetSeen()
    {
        if (activeCamera == null || targetObject == null)
            return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(activeCamera);
        Collider targetCollider = targetObject.GetComponent<Collider>();

        if (targetCollider != null)
        {
            return GeometryUtility.TestPlanesAABB(planes, targetCollider.bounds);
        }

        Renderer targetRenderer = targetObject.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            return GeometryUtility.TestPlanesAABB(planes, targetRenderer.bounds);
        }

        return false;
    }
}