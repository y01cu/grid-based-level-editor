using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGrid
{
    private GridSystem<TilemapObject> gridSystem;

    public TilemapGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        gridSystem = new GridSystem<TilemapObject>(width, height, cellSize, originPosition, (GridSystem<TilemapObject> g, int x, int y) => new TilemapObject(g, x, y));
    }

    public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite)
    {
        TilemapObject tilemapObject = gridSystem.GetGridObject(worldPosition);
        tilemapObject?.SetTilemapSprite(tilemapSprite);
    }

    public void SetTilemapVisualGrid(TilemapVisual tilemapVisual)
    {
        tilemapVisual.SetGridSystem(gridSystem);
    }


    public class TilemapObject
    {
        public enum TilemapSprite
        {
            None,
            Ground,
            Frog
        }

        private GridSystem<TilemapObject> gridSystem;
        private int x;
        private int y;
        private TilemapSprite tilemapSprite;

        public TilemapObject(GridSystem<TilemapObject> gridSystem, int x, int y)
        {
            this.gridSystem = gridSystem;
            this.x = x;
            this.y = y;
        }

        public TilemapSprite GetTilemapSprite()
        {
            return tilemapSprite;
        }

        public void SetTilemapSprite(TilemapSprite tilemapSprite)
        {
            this.tilemapSprite = tilemapSprite;
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return tilemapSprite.ToString();
        }
    }
}