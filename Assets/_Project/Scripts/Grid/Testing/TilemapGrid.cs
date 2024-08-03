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

    public void SetupTilemapOnPosition(Vector3 worldPosition, TilemapObject.TilemapObjectType tilemapObjectType)
    {
        // TODO: Update this method
        TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(worldPosition);
        tilemapObject?.SetTilemapSpriteAndType(tilemapObjectType);
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

                Debug.Log($"obj material index: {tilemapObject.materialIndex}");

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

    //public void LoadWithCellBasesWithSO(CellBase[] cellBases)
    public void LoadWithCellBasesWithSO()
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

            Debug.Log($"material index: {tilemapObject.materialIndex}");

            //AssignMaterial(cellBases, tilemapObject);

            HandleCellWithTilemapObjectOnPosition(tilemapObject, newPosition);

            //objectToInstantiate = tilemapObject.GetObjectTypeSO().prefab.gameObject;
            //objectToInstantiate.GetComponent<Renderer>().sharedMaterial = tilemapObject.GetObjectTypeSO().normalMaterials[tilemapObject.materialIndex];
            //Object.Instantiate(objectToInstantiate, newPosition, Quaternion.Euler(270, 0, 0));

            var objectName = tilemapObject.GetObjectTypeSO().name;
            int targetIndex = 0;


            // int index = cellBaseObj.CellObjectPrefabs.IndexOf(tilemapObject.GetObjectTypeSO().prefab.gameObject);
            // Debug.Log($"index: {index}");
            // GameObject objectToInstantiate = cellBaseObj.CellObjectPrefabs[index]; 






            //instantiatedCell.GetComponent<Renderer>().sharedMaterials[0] = baseCellSO.normalMaterials[tilemapObject.materialIndex];
            // Object.Instantiate(cellBaseObj, newPos, Quaternion.Euler(270, 0, 0));
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void HandleCellWithTilemapObjectOnPosition(TilemapObject tilemapObject, Vector3 newPosition)
    {
        var initialAngleForCamera = Quaternion.Euler(270, 0, 0);

        var baseCellSO = Resources.Load<ObjectTypeSO>("Cell");
        Debug.Log($"cell so loaded: {baseCellSO}", baseCellSO);
        var instantiatedCell = Object.Instantiate(baseCellSO.prefab, newPosition, initialAngleForCamera);
        //instantiatedCell
        var cellObjectTypeSO = instantiatedCell.GetComponent<CellBase>().objectTypeSO;
        cellObjectTypeSO = tilemapObject.GetObjectTypeSO();
        // previously Quaternion.Euler(270, 0, 0)
        var newObject = Object.Instantiate(cellObjectTypeSO.prefab, newPosition, initialAngleForCamera); 
        newObject.GetComponent<Renderer>().sharedMaterial = tilemapObject.GetObjectTypeSO().normalMaterials[tilemapObject.materialIndex];
        newObject.GetComponent<CellObject>().AdjustTransform();
        var renderer = instantiatedCell.GetComponent<Renderer>();
        var materials = renderer.sharedMaterials;
        materials[0] = baseCellSO.normalMaterials[tilemapObject.materialIndex];
        renderer.sharedMaterials = materials;
    }

    //private GameObject cellToInstantiate;
    private GameObject objectToInstantiate;
    //private void AssignMaterial(CellBase[] cellBases, TilemapObject tilemapObject)
    //{
    //    Debug.Log($"mat i + 1: {tilemapObject.materialIndex + 1}");
    //    cellBaseObj = cellBases[tilemapObject.materialIndex + 1];
    //}
    // public void ChangeObjectSpriteOnPosition(CellBase[] cellBases)
    // {

    // }

    //private CellBase cellBaseObj;

    public void SetObjectColorToInstantiate(CellBase newCellBaseObj)
    {
        Debug.Log($"new cell base obj: {newCellBaseObj}");
        //cellBaseObj = newCellBaseObj;
    }

    public class SaveObject
    {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }

    public class TilemapObject
    {
        [Serializable]
        // public enum TilemapMaterialIndex
        // {
        //     // None = 0,
        //     Blue = 0,
        //     Green = 1,
        //     Red = 2,
        //     Yellow = 3
        // }

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
        // private TilemapMaterialIndex tilemapMaterialIndex;
        private TilemapObjectType tilemapObjectType;
        private ObjectTypeSO objectTypeSO;
        // private Material material;

        public int materialIndex;

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
        public TilemapObjectType GetTilemapObjectType()
        {
            return tilemapObjectType;
        }

        public ObjectTypeSO GetObjectTypeSO()
        {
            return objectTypeSO;
        }

        public void SetTilemapSpriteAndType(TilemapObjectType newType)
        {
            // this.tilemapMaterialIndex = newIndex;
            this.tilemapObjectType = newType;
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
            // return _tilemapSpriteTexture.ToString();
            // return tilemapObjectType.ToString();
            return objectTypeSO == null ? $"" : $"{x} {y} {objectTypeSO.name}";
        }

        [Serializable]
        public class SaveObject
        {
            // [FormerlySerializedAs("tilemapSprite")]
            // public TilemapMaterialIndex tilemapMaterialIndex;

            public TilemapObjectType tilemapObjectType;
            public ObjectTypeSO objectTypeSO;
            // public Material material;

            public int materialIndex;

            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                materialIndex = materialIndex,
                tilemapObjectType = tilemapObjectType,
                objectTypeSO = objectTypeSO,
                // material = material,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            // tilemapMaterialIndex = saveObject.tilemapMaterialIndex;
            materialIndex = saveObject.materialIndex;
            tilemapObjectType = saveObject.tilemapObjectType;
            objectTypeSO = saveObject.objectTypeSO;
            // material = saveObject.material;
        }
    }
}