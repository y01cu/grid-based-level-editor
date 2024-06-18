using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Direction direction;

    private void Start()
    {
        Debug.Log("arrow z: " + (int)transform.rotation.eulerAngles.z  + " | dir: " + direction, this);
    }
}