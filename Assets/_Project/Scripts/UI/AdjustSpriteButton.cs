using System;
using UnityEngine;

public class AdjustSpriteButton : MonoBehaviour
{
    // [SerializeField] private TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture;
    //
    // public static event Action<TilemapGrid.TilemapObject.TilemapSpriteTexture> AdjustSpriteTexture;

    public void UpdateSpriteAndSOColor()
    {
        // AdjustSpriteTexture?.Invoke(tilemapSpriteTexture);
        // OnActiveBuildingColorChanged?.Invoke();
    }
    
    public static EventHandler<OnActiveBuildingColorChangedEventArgs> OnActiveBuildingColorChanged;
}

public class OnActiveBuildingColorChangedEventArgs
{
    public ObjectTypeSO activeObjectTypeSO;
}