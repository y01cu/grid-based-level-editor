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
    }

    public void DeleteLastTilemapObject()
    {
        materialIndexList.Remove(materialIndexList[materialIndexList.Count - 1]);
        objectTypeSOList.Remove(objectTypeSOList[objectTypeSOList.Count - 1]);
        rotationList.Remove(rotationList[rotationList.Count - 1]);
        gridSystem.TriggerGridObjectChanged(x, y);
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

    public SaveObject Save()
    {
        return new SaveObject
        {
            materialIndexList = materialIndexList,
            objectTypeSOList = objectTypeSOList,
            rotationList = rotationList,
            x = x,
            y = y
        };
    }

    public void Load(SaveObject saveObject)
    {
        materialIndexList = saveObject.materialIndexList;
        objectTypeSOList = saveObject.objectTypeSOList;
        rotationList = saveObject.rotationList;
        x = saveObject.x;
        y = saveObject.y;
    }
}
