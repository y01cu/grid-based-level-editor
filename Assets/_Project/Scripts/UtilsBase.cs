using UnityEngine;

public static class UtilsBase
{
    public static Vector3 GetMouseWorldPosition3OnCamera(Camera camera)
    {
        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        return mouseWorldPosition;
    }

    public static Vector2 GetMouseWorldPosition2OnCamera(Camera camera)
    {
        Vector2 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        return mouseWorldPosition;
    }
}