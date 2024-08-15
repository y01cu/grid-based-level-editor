using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    public void SetupTilemapObject(Vector3 worldPosition, Vector3 rotation, ObjectTypeSO objectTypeSO)
    {
        TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(worldPosition);

        tilemapObject?.UpdateTilemapObject(objectTypeSO.materialIndex, objectTypeSO, rotation);
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
                Debug.Log($"object type so list count {tilemapObject.GetObjectTypeSOList().Count}");

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

    public void LoadWithSO()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach (var tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            var tilemapObject = gridSystem.GetGridObjectOnCoordinates(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
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
            if (tilemapObject.GetObjectTypeSOList()[i] == null)
            {
                continue;
            }

            var baseCellSO = Resources.Load<ObjectTypeSO>("Cell");
            var instantiatedCell = Object.Instantiate(baseCellSO.prefab, newPosition + new Vector3(0, i * 0.1f, -i * 0.1f), initialAngleForCamera);
            var currentCellBase = instantiatedCell.GetComponent<CellBase>();
            instantiatedCell.transform.DOScale(instantiatedCell.transform.localScale, 0.5f).From(Vector3.zero);
            currentCellBase.objectTypeSO = tilemapObject.GetObjectTypeSOList()[i];
            currentCellBase.cellObjectMaterialIndex = tilemapObject.GetMaterialIndexList()[i];
            currentCellBase.cellObjectSpawnRotation = tilemapObject.GetRotationList()[i];

            var cellObjectTypeSO = tilemapObject.GetObjectTypeSOList()[i];
            currentCellBase.cellObject = cellObjectTypeSO.prefab.GetComponent<CellObject>();

            var renderer = instantiatedCell.GetComponent<Renderer>();
            var materials = renderer.sharedMaterials;
            // 0 index is what we want to modify
            materials[0] = baseCellSO.normalMaterials[tilemapObject.GetMaterialIndexList()[i]];
            renderer.sharedMaterials = materials;
        }
    }

    public void SetObjectColorToInstantiate(CellBase newCellBaseObj)
    {
        Debug.Log($"new cell base obj: {newCellBaseObj}");
    }

    public class SaveObject
    {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }
}