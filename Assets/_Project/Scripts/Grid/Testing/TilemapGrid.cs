using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TilemapGrid
{
    public event EventHandler OnLoaded;

    public GridSystem<Stack<TilemapObject>> gridSystem { get; private set; }

    public TilemapGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        gridSystem = new GridSystem<Stack<TilemapObject>>(width, height, cellSize, originPosition, (GridSystem<Stack<TilemapObject>> gridSystem, int x, int y) =>
        {
            var stack = new Stack<TilemapObject>();
            stack.Push(new TilemapObject(gridSystem, x, y));
            return stack;
        });
    }

    public void SetupTilemapObject(Vector3 worldPosition, Vector3 rotation, ObjectTypeSO objectTypeSO)
    {

        TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(worldPosition);

        Debug.Log($"tilemap obj pos vec:{tilemapObject.GetPositionVector3()}");

        tilemapObject?.UpdateTilemapObject(objectTypeSO.materialIndex, objectTypeSO, rotation);
        objectStack.Push(tilemapObject);
        LogStackObjectCountWithMessage(objectStack, "new -");
        //LogEveryObjectOfCurrentTileFromTopToBottom(gridSystem.GetGridObjectOnCoordinates(worldPosition));
    }

    private void LogStackObjectCountWithMessage(Stack<TilemapObject> objectStack, string initialMessage)
    {
        int objectCounter = 0;
        foreach (var tilemapObject in objectStack)
        {
            if (tilemapObject.GetObjectTypeSO() != null)
            {
                objectCounter++;

                Debug.Log($"obj pos: {tilemapObject.GetPositionVector3()}");
            }
        }
        Debug.Log($"{initialMessage} obj count: {objectCounter}");
    }

    private void LogEveryObjectOfCurrentTileFromTopToBottom(Stack<TilemapObject> objectStack)
    {
        foreach (var tilemapObject in objectStack)
        {
            Debug.Log(tilemapObject);
        }

        Debug.Log("----------");
    }

    public void SetTilemapVisualGrid(TilemapGrid tilemapGrid, TilemapVisual tilemapVisual)
    {
        tilemapVisual.SetGridSystem(tilemapGrid, gridSystem);
    }

    public void Save()
    {
        List<Stack<TilemapObject.SaveObject>> tilemapObjectSaveObjecStackList = new List<Stack<TilemapObject.SaveObject>>();
        for (int x = 0; x < gridSystem.Width; x++)
        {
            for (int y = 0; y < gridSystem.Height; y++)
            {
                Stack<TilemapObject> tilemapObjectStack = gridSystem.GetGridObjectOnCoordinates(x, y);
                tilemapObjectSaveObjecStackList.Add(tilemapObjectStack.Peek().Save());
                //Debug.Log($"obj material index: {tilemapObject.Peek().GetMaterialIndex()}");
                //Debug.Log(tilemapObject.Peek().GetObjectTypeSO() ? $"saved obj so: {tilemapObject.Peek().GetObjectTypeSO().name}" : $"x: {x}, y: {y} is null ");
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjecStackList.ToArray() };
        SaveSystem.SaveObject(saveObject);
    }

    public void Load()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();

        foreach (var tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            var tilemapObject = gridSystem.GetGridObjectOnCoordinates(tilemapObjectSaveObject.Peek().x, tilemapObjectSaveObject.Peek().y);
            Debug.Log("loaded tilemap object: " + tilemapObject);
            tilemapObject.Peek().Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.Peek().x, tilemapObjectSaveObject.Peek().y);
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoadWithSO()
    {
        
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        for (int i = 0; i < saveObject.tilemapObjectSaveObjectArray.Length; i++)
        {
            var tilemapObjectSaveObject = saveObject.tilemapObjectSaveObjectArray[i].ToArray()[i];
            var tilemapObjectStack = gridSystem.GetGridObjectOnCoordinates(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            foreach (var tilemapObject in tilemapObjectStack)
            {
                tilemapObject.Load(saveObject.tilemapObjectSaveObjectArray[i]);

                //Debug.Log(tilemapObject.GetObjectTypeSO() ? $"loaded obj so: {tilemapObject.GetObjectTypeSO().name}" : $"x: {tilemapObjectSaveObject.x}, y: {tilemapObjectSaveObject.y} is null ");
            }
            //tilemapObject.Peek().Load(tilemapObjectSaveObject);
            gridSystem.TriggerGridObjectChanged(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            var newPosition = new Vector3(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y, 0);

            var isObjectNull = tilemapObject.GetObjectTypeSOList() == null || !tilemapObject.GetObjectTypeSOList().Any();

            if (isObjectNull)
            {
                continue;
            }

            HandleCellWithTilemapObjectOnPosition(tilemapObject, newPosition);
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void HandleCellWithTilemapObjectOnPosition(TilemapObject tilemapObject, Vector3 newPosition)
    {
        var initialAngleForCamera = Quaternion.Euler(270, 0, 0);

        for (int i = 0; i < tilemapObject.GetObjectTypeSOList().Count; i++)
        {
            //tilemapObject.GetObjectTypeSOList()[i].materialIndex = tilemapObject.GetMaterialIndexList()[i];
            //tilemapObject.GetObjectTypeSOList()[i].spawnRotation = tilemapObject.GetRotationList()[i];
            // ---

            var baseCellSO = Resources.Load<ObjectTypeSO>("Cell");
            var instantiatedCell = Object.Instantiate(baseCellSO.prefab, newPosition + new Vector3(0, i * 0.1f, -i * 0.1f), initialAngleForCamera);
            var currentCellBase = instantiatedCell.GetComponent<CellBase>();
            currentCellBase.objectTypeSO = tilemapObject.GetObjectTypeSOList()[i];
            currentCellBase.cellObjectMaterialIndex = tilemapObject.GetMaterialIndexList()[i];
            currentCellBase.cellObjectSpawnRotation = tilemapObject.GetRotationList()[i];


            var cellObjectTypeSO = tilemapObject.GetObjectTypeSOList()[i];
            currentCellBase.cellObject = cellObjectTypeSO.prefab.GetComponent<CellObject>();
            //currentCellBase.cellObject.AdjustTransformForSetup();

            //currentCellBase.objectTypeSO.prefab.GetComponent<Renderer>().sharedMaterial = baseCellSO.normalMaterials[tilemapObject.GetMaterialIndexList()[i]];
            //currentCellBase.objectTypeSO.prefab.GetComponent<CellObject>().AdjustTransformForSetup();

            //var newObject = Object.Instantiate(cellObjectTypeSO.prefab, newPosition + new Vector3(0, i * 0.15f, -i), Quaternion.Euler(tilemapObject.GetRotationList()[i]));
            //newObject.GetComponent<Renderer>().sharedMaterial = cellObjectTypeSO.normalMaterials[tilemapObject.GetMaterialIndexList()[i]];
            //newObject.GetComponent<CellObject>().AdjustTransformForSetup();

            var renderer = instantiatedCell.GetComponent<Renderer>();
            var materials = renderer.sharedMaterials;
            // 0 index is what we want to modify
            materials[0] = baseCellSO.normalMaterials[tilemapObject.GetMaterialIndexList()[i]];
            renderer.sharedMaterials = materials;
        }
    }

    private GameObject objectToInstantiate;

    public void SetObjectColorToInstantiate(CellBase newCellBaseObj)
    {
        Debug.Log($"new cell base obj: {newCellBaseObj}");
    }

    public class SaveObject
    {
        public Stack<TilemapObject.SaveObject>[] tilemapObjectSaveObjectArray;
    }

    public class TilemapObject
    {
        private GridSystem<TilemapObject> gridSystem;
        private List<ObjectTypeSO> objectTypeSOList;
        private List<Vector3> rotationList;
        private List<int> materialIndexList;
        private int x;
        private int y;

        public TilemapObject(GridSystem<Stack<TilemapObject>> gridSystem, int x, int y)
        {
            this.gridSystem = gridSystem;
            this.x = x;
            this.y = y;
            objectTypeSOList = new List<ObjectTypeSO>();
            rotationList = new List<Vector3>();
            materialIndexList = new List<int>();
        }

        public Vector3 GetPositionVector3()
        {
            return new Vector3(x, y);
        }
        public List<Vector3> GetRotationList()
        {
            return rotationList;
        }

        public List<ObjectTypeSO> GetObjectTypeSOList()
        {
            return objectTypeSOList;
        }

        public List<int> GetMaterialIndexList()
        {
            return materialIndexList;
        }
        public void SetTilemapSpriteAndType() // can have parameters for setting something on the tile
        {
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public void UpdateTilemapObject(int newMaterialIndex, ObjectTypeSO newObjectTypeSO, Vector3 newRotation)
        {
            materialIndexList.Add(newMaterialIndex);
            objectTypeSOList.Add(newObjectTypeSO);
            rotationList.Add(newRotation);
            gridSystem.TriggerGridObjectChanged(x, y);
            Debug.Log($"material index: {newMaterialIndex} | or: {newObjectTypeSO.materialIndex}");
        }

        public void ClearSO()
        {
            objectTypeSOList.Clear();
            rotationList.Clear();
            materialIndexList.Clear();
            gridSystem.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return objectTypeSOList.Count == 0 ? $"" : $"{x} {y} {string.Join(", ", objectTypeSOList.Select(o => o.name))}";
        }

        [Serializable]
        public class SaveObject
        {
            public List<ObjectTypeSO> objectTypeSOList;
            public List<Vector3> rotationList;
            public List<int> materialIndexList;
            public int x;
            public int y;
        }

        /// <summary>
        /// Returns a stack of save objects on that grid object position
        /// </summary>
        /// <returns></returns>
        public Stack<SaveObject> Save()
        {
            Stack<SaveObject> saveObjectStack = new Stack<SaveObject>();

            //Debug.Log($"gs obj: {gridSystem.GetGridObjectOnCoordinates(x, y)}");
            for (int i = 0; i < gridSystem.GetGridObjectOnCoordinates(x, y).Count; i++)
            {
                materialIndexList = materialIndexList,
                objectTypeSOList = objectTypeSOList,
                rotationList = rotationList,
                x = x,
                y = y
            };
        }

        public void Load(Stack<SaveObject> saveObjectStack)
        {
            materialIndexList = saveObject.materialIndexList;
            objectTypeSOList = saveObject.objectTypeSOList;
            rotationList = saveObject.rotationList;
            x = saveObject.x;
            y = saveObject.y;
        }
    }
}