using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DetectedObjectStorage{
    public List<Vector3> points = new();

    public List<GameObject> detectedObjects = new();

    public List<Berry> detectedBerries = new();
    
    public void ClearLists()
    {
        points.Clear();
        detectedObjects.Clear();
        detectedBerries.Clear();
    }
}