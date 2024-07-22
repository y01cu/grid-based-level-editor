using UnityEngine;

public static class UtilsBase
{
    public static Vector3 GetMouseWorldPositionOnCamera(Camera camera)
    {
        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        return mouseWorldPosition;
    }
}