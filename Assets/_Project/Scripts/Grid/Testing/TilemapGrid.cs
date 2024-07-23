using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class TilemapGrid
{
    public event EventHandler OnLoaded;

    public GridSystem<TilemapObject> gridSystem { get; private set; }

    public TilemapGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        gridSystem = new GridSystem<TilemapObject>(width, height, cellSize, originPosition, (GridSystem<TilemapObject> g, int x, int y) => new TilemapObject(g, x, y));
    }

    public void SetupTilemapOnPosition(Vector3 worldPosition, TilemapObject.TilemapSpriteTexture tilemapSpriteTexture, TilemapObject.TilemapObjectType tilemapObjectType)
    {
        TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(worldPosition);
        tilemapObject?.SetTilemapSpriteAndType(tilemapSpriteTexture, tilemapObjectType);
    }

    public void SetupTilemapOnPositionWithSO(Vector3 worldPosition, TilemapObject.TilemapSpriteTexture tilemapSpriteTexture, ObjectTypeSO objectTypeSO)
    {
        TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(worldPosition);
        tilemapObject?.SetTilemapSpriteAndSO(tilemapSpriteTexture, objectTypeSO);
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
                TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(x, y);

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
            var tilemapObject = gridSystem.GetGridObjectOnCoordinates(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }


    public void LoadWithCellBasesWithSO(CellBase[] cellBases)
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();

        foreach (var tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            var tilemapObject = gridSystem.GetGridObjectOnCoordinates(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            var newPos = new Vector3(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y, 0);
            if (tilemapObject.GetTilemapSprite() == TilemapObject.TilemapSpriteTexture.None)
            {
                continue;
            }

            ChangeObjectSpriteOnPosition(cellBases, tilemapObject.GetTilemapSprite());

            Object.Instantiate(cellBaseObj, newPos, Quaternion.Euler(270, 0, 0));
        }


        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeObjectSpriteOnPosition(CellBase[] cellBases, TilemapObject.TilemapSpriteTexture tilemapSpriteTexture)
    {
        var objectIndex = tilemapSpriteTexture switch
        {
            TilemapObject.TilemapSpriteTexture.Blue => 1,
            TilemapObject.TilemapSpriteTexture.Green => 2,
            TilemapObject.TilemapSpriteTexture.Red => 3,
            TilemapObject.TilemapSpriteTexture.Yellow => 4,
        };

        SetObjectToInstantiate(cellBases[objectIndex]);
    }

    private CellBase cellBaseObj;

    public void SetObjectToInstantiate(CellBase newCellBaseObj)
    {
        cellBaseObj = newCellBaseObj;
    }

    public class SaveObject
    {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }


    public class TilemapObject
    {
        [Serializable]
        public enum TilemapSpriteTexture
        {
            None = 0,
            Blue = 1,
            Green = 2,
            Red = 3,
            Yellow = 4
        }

        public enum TilemapObjectType
        {
            None = 0,
            Arrow = 1,
            Berry = 2,
            Frog = 3
        }

        private GridSystem<TilemapObject> gridSystem;
        private int x;
        private int y;
        private TilemapSpriteTexture tilemapSpriteTexture;
        private TilemapObjectType tilemapObjectType;
        private ObjectTypeSO objectTypeSO;

        public TilemapObject(GridSystem<TilemapObject> gridSystem, int x, int y)
        {
            this.gridSystem = gridSystem;
            this.x = x;
            this.y = y;
        }

        public Vector3 GetPositionVector3()
        {
            return new Vector3(x, y);
        }

        public TilemapSpriteTexture GetTilemapSprite()
        {
            return tilemapSpriteTexture;
        }

        public TilemapObjectType GetTilemapObjectType()
        {
            return tilemapObjectType;
        }

        public ObjectTypeSO GetObjectTypeSO()
        {
            return objectTypeSO;
        }

        public void SetTilemapSpriteAndType(TilemapSpriteTexture newTexture, TilemapObjectType newType)
        {
            this.tilemapSpriteTexture = newTexture;
            this.tilemapObjectType = newType;
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public void SetTilemapSpriteAndSO(TilemapSpriteTexture newTexture, ObjectTypeSO newObjectTypeSO)
        {
            tilemapSpriteTexture = newTexture;
            objectTypeSO = newObjectTypeSO;
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            // return _tilemapSpriteTexture.ToString();
            // return tilemapObjectType.ToString();
            return objectTypeSO == null ? "none" : objectTypeSO.name;
        }

        [Serializable]
        public class SaveObject
        {
            [FormerlySerializedAs("tilemapSprite")]
            public TilemapSpriteTexture tilemapSpriteTexture;

            public TilemapObjectType tilemapObjectType;
            public ObjectTypeSO objectTypeSO;

            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSpriteTexture = tilemapSpriteTexture,
                tilemapObjectType = tilemapObjectType,
                objectTypeSO = objectTypeSO,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            tilemapSpriteTexture = saveObject.tilemapSpriteTexture;
            tilemapObjectType = saveObject.tilemapObjectType;
            objectTypeSO = saveObject.objectTypeSO;
        }
    }
}