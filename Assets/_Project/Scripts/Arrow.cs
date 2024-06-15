using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Direction direction;

    public enum Direction
    {
        _None,
        Up,
        Down,
        Right,
        Left,
    }

    private void Start()
    {
        Debug.Log("arrow z: " + (int)transform.rotation.eulerAngles.z, this);
    }
}