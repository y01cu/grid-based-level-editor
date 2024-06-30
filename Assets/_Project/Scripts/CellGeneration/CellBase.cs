using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class CellBase : MonoBehaviour
{
    // ATTENTION: Always sort array objects in alphabetical order

    public GameObject[] CellObjectPrefabs;
    // public GameObject cellObject;

    public ObjectColor objectColor;
    public ObjectType objectType;

    private const float InitialWaitingTime = 2f;
    private const float DestructionWaitingTime = 3.5f;
    private bool isObjectSpawned;
    private float timer;

    public Direction arrowDirection;

    private bool isDeathOrderGiven;

    public Vector3 additionalRotationVector3;

    public OrderType orderType;

    [SerializeField] private Transform objectTargetTransformFromChild;

    // [SerializeField] private Transform parentTransform;

    [SerializeField] private LayerMask collisionLayers;

    private bool isSpawningFinalBerryForFrog;

    public bool IsSpawningFinalBerryForFrog()
    {
        return isSpawningFinalBerryForFrog;
    }

    public void SetAsSpawningFinalBerryForFrog()
    {
        isSpawningFinalBerryForFrog = true;
    }

    private void Start()
    {
        if (objectColor != ObjectColor._Empty)
        {
            LevelManager.Instance.IncreaseActiveNonGrayCellCount();
        }
    }

    public GameObject CreateCellObject()
    {
        var cellObject = Instantiate(CellObjectPrefabs[(int)objectType]);
        cellObject.transform.position = new Vector3(0, 0, 0);
        cellObject.transform.DOScale(Vector3.zero, 2f).From();
        cellObject.transform.localPosition = objectTargetTransformFromChild.position;
        cellObject.transform.Rotate(additionalRotationVector3);

        if (objectType == ObjectType.Arrow)
        {
            var arrowObject = cellObject.GetComponent<Arrow>();
            arrowObject.SetDirection(arrowDirection);
            SetUpCellObject(arrowObject);
        }

        else if (objectType == ObjectType.Frog)
        {
            var frogObject = cellObject.GetComponentInChildren<Frog>();
            frogObject.SetOrderType(orderType);
            SetUpCellObject(frogObject);
        }

        else if (objectType == ObjectType.Berry)
        {
            var berryObject = cellObject.GetComponent<CellObject>();
            SetUpCellObject(berryObject);
        }

        return cellObject;
    }

    private void SetUpCellObject(CellObject cellObject)
    {
        cellObject.objectColor = objectColor;
        cellObject.objectType = objectType;
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

        timer += Time.deltaTime;

        if (timer >= InitialWaitingTime && !isObjectSpawned)
        {
            Vector3 rayDirection = transform.up;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, RayLength) && hitInfo.collider.gameObject.CompareTag("Cell"))
            {
                Debug.DrawRay(transform.position, rayDirection * RayLength, Color.green);
            }
            else
            {
                isObjectSpawned = true;
                var cellObject = CreateCellObject();
                timer = 0;

                if (isSpawningFinalBerryForFrog)
                {
                    cellObject.GetComponent<Berry>().SetAsLastBerryForFrog();
                }
            }
        }

        if (timer >= DestructionWaitingTime && isObjectSpawned && !isDeathOrderGiven)
        {
            Vector3 rayDirection = transform.up;

            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hitInfo, RayLength * 2.75f, collisionLayers))
            {
                Debug.DrawRay(transform.position, rayDirection * RayLength * 2.75f, Color.green);
            }
            else
            {
                isDeathOrderGiven = true;
                transform.DOScale(Vector3.zero, 1f).onComplete += () =>
                {
                    LevelManager.Instance.DecreaseActiveNonGrayCellCount();
                    Destroy(gameObject);
                };
            }
        }
    }
}