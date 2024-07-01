using DG.Tweening;
using UnityEngine;

public class CellObjectFactory
{
    private readonly GameObject[] cellObjectPrefabs;
    private readonly Transform objectTargetTransformFromChild;

    public CellObjectFactory(GameObject[] cellObjectPrefabs, Transform objectTargetTransformFromChild)
    {
        this.cellObjectPrefabs = cellObjectPrefabs;
        this.objectTargetTransformFromChild = objectTargetTransformFromChild;
    }

    public GameObject CreateCellObject(ObjectType objectType, Vector3 additionalRotationVector3, Direction arrowDirection, OrderType orderType, ObjectColor objectColor)
    {
        // var cellObject = Object.Instantiate(cellObjectPrefabs[(int)objectType], objectTargetTransformFromChild.position, Quaternion.identity);
        var cellObject = Object.Instantiate(cellObjectPrefabs[(int)objectType]);
        cellObject.transform.position = new Vector3(0, 0, 0);
        cellObject.transform.localPosition = objectTargetTransformFromChild.position;
        cellObject.transform.DOScale(Vector3.zero, 2f).From();
        cellObject.transform.Rotate(additionalRotationVector3);

        var cellObjectComponent = GrabCellObjectBasedOnObjectType(objectType, cellObject);
        cellObjectComponent.Initialize(objectColor, objectType);

        if (cellObjectComponent is Arrow arrow)
        {
            arrow.SetDirection(arrowDirection);
        }
        else if (cellObjectComponent is Frog frog)
        {
            frog.SetOrderType(orderType);
        }

        return cellObject;
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