using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;

public class TilemapGrid
{
    public event EventHandler OnLoaded;

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

    public void SetTilemapVisualGrid(TilemapGrid tilemapGrid, TilemapVisual tilemapVisual)
    {
        tilemapVisual.SetGridSystem(tilemapGrid, gridSystem);
    }

    public void Save()
    {
        List<TilemapObject.SaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.SaveObject>();
        for (int x = 0; x < gridSystem.Width; x++)
        {
            for (int y = 0; y < gridSystem.Height; y++)
            {
                TilemapObject tilemapObject = gridSystem.GetGridObject(x, y);

                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(saveObject);
    }

    public void Load()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();

        foreach (var tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            var tilemapObject = gridSystem.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            Debug.Log($"loaded tilemap obj: {tilemapObject} | on x: {tilemapObjectSaveObject.x} y: {tilemapObjectSaveObject.y}");
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoadWithCellBases(CellBase[] cellBases)
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();

        foreach (var tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            var tilemapObject = gridSystem.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            Debug.Log($"loaded tilemap obj: {tilemapObject} | on x: {tilemapObjectSaveObject.x} y: {tilemapObjectSaveObject.y}");
            var newPos = new Vector3(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y, 0);
            if (tilemapObject.GetTilemapSprite() == TilemapObject.TilemapSprite.None)
            {
                continue;
            }

            CreateObjectBasedOnSpriteWithPosition(cellBases, tilemapObject.GetTilemapSprite(), newPos);
        }


        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void CreateObjectBasedOnSpriteWithPosition(CellBase[] cellBases, TilemapObject.TilemapSprite tilemapSprite, Vector3 pos)
    {
        if (tilemapSprite == TilemapObject.TilemapSprite.Blue)
        {
            SetObjectToInstantiate(cellBases[1].gameObject);
        }

        if (tilemapSprite == TilemapObject.TilemapSprite.Green)
        {
            SetObjectToInstantiate(cellBases[2].gameObject);
        }

        if (tilemapSprite == TilemapObject.TilemapSprite.Red)
        {
            SetObjectToInstantiate(cellBases[3].gameObject);
        }

        if (tilemapSprite == TilemapObject.TilemapSprite.Yellow)
        {
            SetObjectToInstantiate(cellBases[4].gameObject);
        }

        GameObject.Instantiate(gameObj, pos, Quaternion.Euler(270, 0, 0));
    }

    private GameObject gameObj;

    public void SetObjectToInstantiate(GameObject newGameObject)
    {
        gameObj = newGameObject;
    }

    public class SaveObject
    {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }


    public class TilemapObject
    {
        [Serializable]
        public enum TilemapSprite
        {
            None,
            Blue,
            Green,
            Red,
            Yellow
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

        [Serializable]
        public class SaveObject
        {
            public TilemapSprite tilemapSprite;
            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSprite = tilemapSprite,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            tilemapSprite = saveObject.tilemapSprite;
        }
    }
}