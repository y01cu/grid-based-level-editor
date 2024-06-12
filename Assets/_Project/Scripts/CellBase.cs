using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;

public class CellBase : MonoBehaviour
{
    // ATTENTION: Always sort array objects in alphabetical order

    public GameObject[] CellObjectPrefabs;
    // public GameObject cellObject;

    public ObjectColor objectColor;
    public ObjectType objectType;

    private const float InitialWaitingTime = 1f;
    private bool _isObjectSpawned;
    private float _timer;

    public Vector3 additionalRotationVector3;

    [SerializeField] private Transform objectTargetTransformFromChild;

    // [SerializeField] private Transform parentTransform;

    public void CreateCellObject()
    {
        var cellObject = Instantiate(CellObjectPrefabs[(int)objectType]);
        cellObject.transform.localPosition = objectTargetTransformFromChild.position;
        cellObject.transform.Rotate(additionalRotationVector3);

        if (objectType == ObjectType.Frog)
        {
        }
        
        // cellObject.transform.position = objectTargetTransformFromChild.position;
    }

    public void ChangeCellObjectType(ObjectType newObjectType)
    {
        objectType = newObjectType;
    }

    private const float RayLength = 0.2f;

    private void Update()
    {
        if (objectColor == ObjectColor._Empty)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= InitialWaitingTime && !_isObjectSpawned)
        {
            Vector3 rayDirection = transform.up;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, RayLength) && hitInfo.collider.gameObject.CompareTag("Cell"))
            {
                // Debug.DrawRay(transform.position, rayDirection * RayLength, Color.green);
            }
            else
            {
                _isObjectSpawned = true;
                CreateCellObject();
                // Debug.DrawRay(transform.position, rayDirection * RayLength, Color.red);
            }
        }
    }

    public enum ObjectColor
    {
        _Empty,
        Blue,
        Green,
        Red,
        Yellow,
    }

    public enum ObjectType
    {
        Arrow,
        Berry,
        Frog,
    }

    // /// <summary>
    // /// Must be called whenever a cell is created
    // /// </summary>
    // /// <param name="targetObjectType"></param>
    // /// <param name="position"></param>
    // /// <param name="rotation"></param>
    // public GameObject CreateCellObjectOfTypeInTransformOnParent(ObjectType targetObjectType, Vector3 position, Quaternion rotation)
    // {
    //     cellObject = CellObjectPrefabs[(int)targetObjectType];
    //     Instantiate(cellObject);
    //     cellObject.transform.localPosition = position;
    //     cellObject.transform.localRotation = rotation;
    //     cellObject.name = targetObjectType + "---";
    //
    //     return cellObject;
    //     // cellObject.transform.SetParent(_parentTransform);
    // }
}