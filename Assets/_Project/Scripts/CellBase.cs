using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
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
    private const float DestructionWaitingTime = 2.5f;
    private bool _isObjectSpawned;
    private float _timer;

    public Direction arrowDirection;

    private bool isDeathOrderGiven;

    public Vector3 additionalRotationVector3;

    [SerializeField] private Transform objectTargetTransformFromChild;

    // [SerializeField] private Transform parentTransform;

    [SerializeField] private LayerMask collisionLayers;

    private void Start()
    {
        if (objectColor != ObjectColor._Empty)
        {
            LevelManager.Instance.IncreaseActiveNonGrayCellCount();
            LevelManager.Instance.LogActiveNonGrayCellCount();
        }
    }

    public void CreateCellObject()
    {
        var cellObject = Instantiate(CellObjectPrefabs[(int)objectType]);
        cellObject.transform.position = new Vector3(0, 0, 0);
        cellObject.transform.DOScale(Vector3.zero, 1f).From();
        cellObject.transform.localPosition = objectTargetTransformFromChild.position;
        cellObject.transform.Rotate(additionalRotationVector3);

        if (objectType == ObjectType.Arrow)
        {
            cellObject.GetComponent<Arrow>().direction = arrowDirection;
        }

        // cellObject.transform.position = objectTargetTransformFromChild.position;
    }

    public void ChangeCellObjectType(ObjectType newObjectType)
    {
        objectType = newObjectType;
    }

    private const float RayLength = 0.2f;

    private void FixedUpdate()
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
                Debug.DrawRay(transform.position, rayDirection * RayLength, Color.green);
            }
            else
            {
                _isObjectSpawned = true;
                CreateCellObject();
                _timer = 0;
                // Debug.DrawRay(transform.position, rayDirection * RayLength, Color.red);
            }
        }


        if (_timer >= DestructionWaitingTime && _isObjectSpawned && !isDeathOrderGiven)
        {
            Vector3 rayDirection = transform.up;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, RayLength * 2.75f, collisionLayers))
            {
                Debug.DrawRay(transform.position, rayDirection * RayLength * 2.75f, Color.green);
            }
            else
            {
                isDeathOrderGiven = true;
                transform.DOScale(Vector3.zero, .5f).onComplete += () =>
                {
                    LevelManager.Instance.DecreaseActiveNonGrayCellCount();
                    LevelManager.Instance.LogActiveNonGrayCellCount();
                    Destroy(gameObject);
                };
            }
        }
    }

    // public 

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