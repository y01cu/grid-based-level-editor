using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    
    public GridSystem(int width, int height)
    {
        this.width = width;
        this.height = height;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Debug.DrawLine();
            }
        }
    }
    
}
