using Mono.CSharp;
using System;
using System.Collections.Generic;
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
        var objectStack = gridSystem.GetGridObjectOnCoordinates(worldPosition);
        LogStackObjectCountWithMessage(objectStack, "prev -");
        //TilemapObject tilemapObject = objectStack.Peek();
        TilemapObject tilemapObject = new TilemapObject(gridSystem, (int)worldPosition.x, (int)worldPosition.y);
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

            bool isObjectNull = tilemapObjectStack.Peek().GetObjectTypeSO() == null;
            if (isObjectNull)
            {
                continue;
            }

            Debug.Log(tilemapObjectStack.Peek().GetObjectTypeSO() ? $"loaded obj so: {tilemapObjectStack.Peek().GetObjectTypeSO().name}" : $"x: {tilemapObjectSaveObject.x}, y: {tilemapObjectSaveObject.y} is null ");

            Debug.Log($"material index: {tilemapObjectStack.Peek().GetMaterialIndex()}");

            HandleCellWithTilemapObjectOnPosition(tilemapObjectStack.Peek(), newPosition);

            var objectName = tilemapObjectStack.Peek().GetObjectTypeSO().name;
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
        //var newObject = Object.Instantiate(cellObjectTypeSO.prefab, newPosition, initialAngleForCamera);
        var newObject = Object.Instantiate(cellObjectTypeSO.prefab, newPosition, Quaternion.Euler(tilemapObject.GetRotation()));
        newObject.GetComponent<Renderer>().sharedMaterial = tilemapObject.GetObjectTypeSO().normalMaterials[tilemapObject.GetMaterialIndex()];
        newObject.GetComponent<CellObject>().AdjustTransformForSetup();
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
        public Stack<TilemapObject.SaveObject>[] tilemapObjectSaveObjectArray;
    }

    public class TilemapObject
    {
        private GridSystem<Stack<TilemapObject>> gridSystem;
        private ObjectTypeSO objectTypeSO;
        private Vector3 rotation;
        private int x;
        private int y;
        private int materialIndex;

        public TilemapObject(GridSystem<Stack<TilemapObject>> gridSystem, int x, int y)
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

        public void UpdateTilemapObject(int newMaterialIndex, ObjectTypeSO newObjectTypeSO, Vector3 newRotation)
        {
            materialIndex = newMaterialIndex;
            objectTypeSO = newObjectTypeSO;
            rotation = newRotation;
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
            public Vector3 rotation;
            public int materialIndex;
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
                var tilemapObject = gridSystem.GetGridObjectOnCoordinates(x, y).ToArray()[i];
                saveObjectStack.Push(new SaveObject
                {
                    materialIndex = tilemapObject.GetMaterialIndex(),
                    objectTypeSO = tilemapObject.GetObjectTypeSO(),
                    rotation = tilemapObject.GetRotation(),
                    x = x,
                    y = y
                });
            }

            return saveObjectStack;
        }

        public void Load(Stack<SaveObject> saveObjectStack)
        {
            var tilemapObjectStack = gridSystem.GetGridObjectOnCoordinates(x, y);
            tilemapObjectStack.ToArray();
            
            for(int i = 0; i < saveObjectStack.Count; i++)
            {
                tilemapObjectStack.ToArray()[i].materialIndex = saveObjectStack.ToArray()[i].materialIndex;
                tilemapObjectStack.ToArray()[i].objectTypeSO = saveObjectStack.ToArray()[i].objectTypeSO;
                tilemapObjectStack.ToArray()[i].rotation = saveObjectStack.ToArray()[i].rotation;
                tilemapObjectStack.ToArray()[i].x = saveObjectStack.ToArray()[i].x;
                tilemapObjectStack.ToArray()[i].y = saveObjectStack.ToArray()[i].y;
            }
        }
    }
}