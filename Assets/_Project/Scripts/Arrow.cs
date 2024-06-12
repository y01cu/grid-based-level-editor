using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Direction direction;
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }
}
