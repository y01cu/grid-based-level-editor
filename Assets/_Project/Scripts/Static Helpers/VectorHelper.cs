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

    /// <summary>
    /// Checks point and updates points dictionary if necessary. 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="pointsDictionary"></param>
    /// <param name="zLayerIncrement"></param>
    /// <returns></returns>
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
}