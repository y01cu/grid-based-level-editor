using System;
using UnityEngine;

public class AdjustSpriteButton : MonoBehaviour
{
    [SerializeField] private TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture;

    public static event Action<TilemapGrid.TilemapObject.TilemapSpriteTexture> AdjustSpriteTexture;

    public void UpdateSprite()
    {
        AdjustSpriteTexture?.Invoke(tilemapSpriteTexture);
    }
}