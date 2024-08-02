using System.Collections.Generic;
using UnityEngine;

public static class VectorHelper
{
    public static Vector3 GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }

    public static Vector3 CheckGivenDictionaryAndUpdateVector(Vector3 point, Dictionary<Vector3, bool> pointsDictionary, float zLayerIncrement)
    {
        if (pointsDictionary.ContainsKey(point))
        {
            point += new Vector3(0, 0, zLayerIncrement);
            return CheckGivenDictionaryAndUpdateVector(point, pointsDictionary, zLayerIncrement);
        }

        pointsDictionary.Add(point, true);
        return point;
    }

    public static bool CheckRaycastUp(float length, Transform transform, LayerMask layers)
    {
        return Physics.Raycast(transform.position, transform.up, length, layers);
    }

    public static RaycastHit[] GetRaycastHitsFromMousePosition(Camera camera)
    {
        RaycastHit[] hits;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray);
        return hits;
    }
}