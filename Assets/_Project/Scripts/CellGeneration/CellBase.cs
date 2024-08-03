using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CellBase : MonoBehaviour
{
    public ObjectTypeSO objectTypeSO;

    public List<GameObject> CellObjectPrefabs;
    public ObjectColor objectColor;
    public ObjectType objectType;
    public Direction arrowDirection;
    public Vector3 additionalRotationVector3;
    public OrderType orderType;

    [SerializeField] private Transform objectTargetTransformFromChild;
    [SerializeField] private LayerMask collisionLayers;

    private const float InitialWaitingTime = 2f;
    private const float DestructionWaitingTime = 3.5f;
    private const float RayLength = 0.2f;
    private float timer;
    private bool isObjectSpawned;
    private bool isDeathOrderGiven;
    private bool isSpawningFinalBerryForFrog;

    private CellObjectFactory cellObjectFactory;

    private void Awake()
    {
        cellObjectFactory = new CellObjectFactory(CellObjectPrefabs, objectTargetTransformFromChild);
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

    private void FixedUpdate()
    {
        //if (objectColor == ObjectColor._Empty)
        //{
        //    return;
        //}

        //timer += Time.deltaTime;

        //if (timer >= InitialWaitingTime && !isObjectSpawned)
        //{
        //    TrySpawnObject();
        //}

        if (timer >= DestructionWaitingTime && isObjectSpawned && !isDeathOrderGiven)
        {
            TryDestroyObject();
        }
    }

    private void TryDestroyObject()
    {
        if (!VectorHelper.CheckRaycastUp(RayLength * 2.75f, transform, collisionLayers))
        {
            isDeathOrderGiven = true;
            transform.DOScale(Vector3.zero, 1f).onComplete += () =>
            {
                LevelManager.Instance.DecreaseActiveNonGrayCellCount();
                Destroy(gameObject);
            };
        }
    }

    private void TrySpawnObject()
    {
        if (!VectorHelper.CheckRaycastUp(RayLength, transform, collisionLayers))
        {
            isObjectSpawned = true;
            var cellObject = cellObjectFactory.CreateCellObject(objectType, additionalRotationVector3, arrowDirection, orderType, objectColor);
            timer = 0;

            if (isSpawningFinalBerryForFrog)
            {
                cellObject.GetComponent<Berry>().SetAsLastBerryForFrog();
            }
        }
    }

    public void ChangeCellObjectType(ObjectType newObjectType)
    {
        objectType = newObjectType;
    }
}