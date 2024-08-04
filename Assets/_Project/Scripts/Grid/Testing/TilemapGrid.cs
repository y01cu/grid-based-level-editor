using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class TilemapGrid
{
    public event EventHandler OnLoaded;

    public GridSystem<TilemapObject> gridSystem { get; private set; }

    public TilemapGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        gridSystem = new GridSystem<TilemapObject>(width, height, cellSize, originPosition, (GridSystem<TilemapObject> g, int x, int y) => new TilemapObject(g, x, y));
    }

    public void SetupTilemapOnPositionWithSO(Vector3 worldPosition, ObjectTypeSO objectTypeSO)
    {
        TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(worldPosition);
        tilemapObject?.UpdateTilemapSpriteAndSOAndMaterial(objectTypeSO.materialIndex, objectTypeSO);
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
                Debug.Log($"obj material index: {tilemapObject.GetMaterialIndex()}");
                Debug.Log(tilemapObject.GetObjectTypeSO() ? $"saved obj so: {tilemapObject.GetObjectTypeSO().name}" : $"x: {x}, y: {y} is null ");
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
            Debug.Log("loaded tilemap object: " + tilemapObject);
            tilemapObject.Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoadWithSO()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach (var tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            var tilemapObject = gridSystem.GetGridObjectOnCoordinates(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            var newPosition = new Vector3(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y, 0);

            bool isObjectNull = tilemapObject.GetObjectTypeSO() == null;
            if (isObjectNull)
            {
                continue;
            }

            Debug.Log(tilemapObject.GetObjectTypeSO() ? $"loaded obj so: {tilemapObject.GetObjectTypeSO().name}" : $"x: {tilemapObjectSaveObject.x}, y: {tilemapObjectSaveObject.y} is null ");

            Debug.Log($"material index: {tilemapObject.GetMaterialIndex()}");

            HandleCellWithTilemapObjectOnPosition(tilemapObject, newPosition);

            var objectName = tilemapObject.GetObjectTypeSO().name;
            int targetIndex = 0;
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void HandleCellWithTilemapObjectOnPosition(TilemapObject tilemapObject, Vector3 newPosition)
    {
        var initialAngleForCamera = Quaternion.Euler(270, 0, 0);

        var baseCellSO = Resources.Load<ObjectTypeSO>("Cell");
        Debug.Log($"cell so loaded: {baseCellSO}", baseCellSO);
        var instantiatedCell = Object.Instantiate(baseCellSO.prefab, newPosition, initialAngleForCamera);
        var cellObjectTypeSO = instantiatedCell.GetComponent<CellBase>().objectTypeSO;
        cellObjectTypeSO = tilemapObject.GetObjectTypeSO();
        var newObject = Object.Instantiate(cellObjectTypeSO.prefab, newPosition, initialAngleForCamera);
        newObject.GetComponent<Renderer>().sharedMaterial = tilemapObject.GetObjectTypeSO().normalMaterials[tilemapObject.GetMaterialIndex()];
        newObject.GetComponent<CellObject>().AdjustTransform();
        var renderer = instantiatedCell.GetComponent<Renderer>();
        var materials = renderer.sharedMaterials;
        materials[0] = baseCellSO.normalMaterials[tilemapObject.GetMaterialIndex()];
        renderer.sharedMaterials = materials;
    }

    private GameObject objectToInstantiate;

    public void SetObjectColorToInstantiate(CellBase newCellBaseObj)
    {
        Debug.Log($"new cell base obj: {newCellBaseObj}");
    }

    public class SaveObject
    {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }

    public class TilemapObject
    {
        private GridSystem<TilemapObject> gridSystem;
        private ObjectTypeSO objectTypeSO;
        private Vector3 rotation;
        private int x;
        private int y;
        private int materialIndex;

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
        public Vector3 GetRotation()
        {
            return rotation;
        }

        public ObjectTypeSO GetObjectTypeSO()
        {
            return objectTypeSO;
        }

        public int GetMaterialIndex()
        {
            return materialIndex;
        }
        public void SetTilemapSpriteAndType() // can have parameters for setting something on the tile
        {
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public void UpdateTilemapSpriteAndSOAndMaterial(int newMaterialIndex, ObjectTypeSO newObjectTypeSO)
        {
            materialIndex = newMaterialIndex;
            objectTypeSO = newObjectTypeSO;
            gridSystem.TriggerGridObjectChanged(x, y);
            Debug.Log($"material index: {materialIndex} | or: {newObjectTypeSO.materialIndex}");
        }

        public void ClearSO()
        {
            objectTypeSO = null;
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return objectTypeSO == null ? $"" : $"{x} {y} {objectTypeSO.name}";
        }

        [Serializable]
        public class SaveObject
        {
            public ObjectTypeSO objectTypeSO;
            public int materialIndex;
            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                materialIndex = materialIndex,
                objectTypeSO = objectTypeSO,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            materialIndex = saveObject.materialIndex;
            objectTypeSO = saveObject.objectTypeSO;
            x = saveObject.x;
            y = saveObject.y;
        }
    }
}