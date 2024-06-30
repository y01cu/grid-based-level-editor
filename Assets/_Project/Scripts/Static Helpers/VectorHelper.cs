using System.Collections;
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
}
