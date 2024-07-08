using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSpriteButton : MonoBehaviour
{
    [SerializeField] private TilemapGrid.TilemapObject.TilemapSprite tilemapSprite;

    public static event Action<TilemapGrid.TilemapObject.TilemapSprite> AdjustSpriteToColor;

    public void UpdateSprite()
    {
        AdjustSpriteToColor?.Invoke(tilemapSprite);
    }
}