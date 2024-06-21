using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProperNaming
{
    [SerializeField] private char color;
    [SerializeField] private char type;

    public string GetProperName()
    {
        return color + "_" + type;
    }

    public char GetColor()
    {
        return color;
    }
}