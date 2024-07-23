using System;
using UnityEngine;

public class AdjustTypeButton : MonoBehaviour
{
    public static event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;

    [SerializeField] private ObjectTypeSO objectTypeSO;

    public void UpdateObjectTypeFromButtonSO()
    {
        OnActiveBuildingTypeChanged?.Invoke(this, new OnActiveBuildingTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
    }
}

public class OnActiveBuildingTypeChangedEventArgs
{
    public ObjectTypeSO activeObjectTypeSO;
}