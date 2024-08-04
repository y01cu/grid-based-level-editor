using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CellObjectFactory
{
    private readonly List<GameObject> cellObjectPrefabs;
    private readonly Transform objectTargetTransformFromChild;

    public CellObjectFactory(List<GameObject> cellObjectPrefabs, Transform objectTargetTransformFromChild)
    {
        this.cellObjectPrefabs = cellObjectPrefabs;
        this.objectTargetTransformFromChild = objectTargetTransformFromChild;
    }

    public GameObject CreateCellObject(ObjectType objectType, Vector3 additionalRotationVector3, Direction arrowDirection, OrderType orderType, ObjectColor objectColor)
    {
        var cellObject = Object.Instantiate(cellObjectPrefabs[(int)objectType]);
        var cellObjectComponent = GrabCellObjectBasedOnObjectType(objectType, cellObject);
        if (cellObjectComponent is Arrow arrow)
        {
            arrow.SetDirection(arrowDirection);
        }
        else if (cellObjectComponent is Frog frog)
        {
            frog.SetOrderType(orderType);
        }

        cellObject.transform.position = new Vector3(0, 0, 0);
        cellObject.transform.localPosition = objectTargetTransformFromChild.position;
        cellObject.transform.DOScale(Vector3.zero, 2f).From();
        cellObject.transform.Rotate(additionalRotationVector3);
        cellObjectComponent.Initialize(objectColor);

        return cellObject;
    }

    public GameObject CreateCellObjectWithSO(ObjectTypeSO objectTypeSO)
    {
        var cellObject = Object.Instantiate(objectTypeSO.prefab);
        // objectTypeSO.cellObjectType.

        cellObject.transform.position = new Vector3(0, 0, 0);
        cellObject.transform.localPosition = objectTargetTransformFromChild.position;
        cellObject.transform.DOScale(Vector3.zero, 2f).From();
        // cellObject.transform.Rotate(additionalRotationVector3);
        // cellObjectComponent.Initialize(objectColor, objectType);

        return cellObject.gameObject;
    }

    private CellObject GrabCellObjectBasedOnObjectType(ObjectType objectType, GameObject freshSpawnedObj)
    {
        return objectType switch
        {
            ObjectType.Arrow => freshSpawnedObj.GetComponent<Arrow>(),
            ObjectType.Berry => freshSpawnedObj.GetComponent<Berry>(),
            ObjectType.Frog => freshSpawnedObj.GetComponentInChildren<Frog>()
        };
    }
}