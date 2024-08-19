using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Each tilemap object is a grid object of the TilemapGrid that can have multiple ObjectTypeSOs, rotations, and material indexes.
/// </summary> <summary>
/// 
/// </summary>
public class TilemapObject
{
    private GridSystem<TilemapObject> gridSystem;
    private List<CellBase> cellBaseList;
    private List<ObjectTypeSO> objectTypeSOList;
    private List<Vector3> rotationList;
    private List<int> materialIndexList;
    private int x;
    private int y;

    public TilemapObject(GridSystem<TilemapObject> gridSystem, int x, int y)
    {
        this.gridSystem = gridSystem;
        this.x = x;
        this.y = y;
        cellBaseList = new List<CellBase>();
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

    public List<CellBase> GetCellBaseList()
    {
        return cellBaseList;
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
        var cellBase = ObjectGhost.Instance.SpawnAdjustAndGetPrefabOnPosition(newRotation, objectTypeSOList.Count);
        cellBaseList.Add(cellBase);
        gridSystem.TriggerGridObjectChanged(x, y);
    }

    public void DeleteLastTilemapObject()
    {
        materialIndexList?.Remove(materialIndexList[materialIndexList.Count - 1]);
        objectTypeSOList?.Remove(objectTypeSOList[objectTypeSOList.Count - 1]);
        rotationList?.Remove(rotationList[rotationList.Count - 1]);
        gridSystem?.TriggerGridObjectChanged(x, y);
        cellBaseList?.Remove(cellBaseList[cellBaseList.Count - 1]);
    }

    public void ClearAllLists()
    {
        objectTypeSOList.Clear();
        rotationList.Clear();
        materialIndexList.Clear();
        if (cellBaseList.Count != 0)
        {
            foreach (CellBase cellBase in cellBaseList)
            {
                Debug.Log($"cell base {cellBase.name} is being destroyed soon");
            }
            foreach (CellBase cellBase in cellBaseList)
            {
                UnityEngine.Object.Destroy(cellBase.gameObject);
            }
            cellBaseList.Clear();
        }
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
        public List<CellBase> cellBaseList;
        public int x;
        public int y;
    }

    public SaveObject Save()
    {
        return new SaveObject
        {
            cellBaseList = cellBaseList,
            materialIndexList = materialIndexList,
            objectTypeSOList = objectTypeSOList,
            rotationList = rotationList,
            x = x,
            y = y
        };
    }

    public void Load(SaveObject saveObject)
    {
        cellBaseList = saveObject.cellBaseList;
        materialIndexList = saveObject.materialIndexList;
        objectTypeSOList = saveObject.objectTypeSOList;
        rotationList = saveObject.rotationList;
        x = saveObject.x;
        y = saveObject.y;
    }
}
