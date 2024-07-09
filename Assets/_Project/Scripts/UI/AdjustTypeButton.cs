using System;
using UnityEngine;

public class AdjustTypeButton : MonoBehaviour
{
    public static event Action<TilemapGrid.TilemapObject.TilemapObjectType> AdjustTileObjectType;

    [SerializeField] private TilemapGrid.TilemapObject.TilemapObjectType tilemapObjectType;


    public void UpdateObjectTypeFromButton()
    {
        AdjustTileObjectType?.Invoke(tilemapObjectType);
    }
}