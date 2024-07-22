using System;
using UnityEngine;

public class AdjustTypeButton : MonoBehaviour
{
    public static event Action<TilemapGrid.TilemapObject.TilemapObjectType> AdjustTileObjectType;
    public static event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;

    [SerializeField] private TilemapGrid.TilemapObject.TilemapObjectType tilemapObjectType;

    [SerializeField] private ObjectTypeSO objectTypeSO;


    public void UpdateObjectTypeFromButton()
    {
        AdjustTileObjectType?.Invoke(tilemapObjectType);
    }

    public void UpdateObjectTypeFromButtonSO()
    {
        OnActiveBuildingTypeChanged?.Invoke(this, new OnActiveBuildingTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
    }
}

public class OnActiveBuildingTypeChangedEventArgs
{
    public ObjectTypeSO activeObjectTypeSO;
}