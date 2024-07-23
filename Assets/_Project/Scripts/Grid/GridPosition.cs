using UnityEngine;

public struct GridPosition
{
    public int x;
    public int y;

    public Vector3 vector3With0Z
    {
        get => new Vector3(x, y);
        set => value = new Vector3(x, y);
    }

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        vector3With0Z = new Vector3(x, y);
    }

    public override string ToString()
    {
        return $"x:{x} y:{y}";
    }
}