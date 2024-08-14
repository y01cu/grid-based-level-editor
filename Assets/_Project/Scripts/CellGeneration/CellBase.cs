using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class CellBase : MonoBehaviour
{
    #region Fields
    public ObjectTypeSO objectTypeSO;
    public CellObject cellObject;
    public int cellObjectMaterialIndex;

    public List<GameObject> CellObjectPrefabs;
    public ObjectColor objectColor;
    public ObjectType objectType;
    public Direction arrowDirection;
    public Vector3 additionalRotationVector3;
    public OrderType orderType;
    public Vector3 cellObjectSpawnRotation;

    [SerializeField] private Transform objectTargetTransformFromChild;
    [SerializeField] private LayerMask collisionLayers;

    private const float InitialWaitingTimeForSpawn = 0.5f;
    private const float InitialWaitingTimeForDestruction = 3.5f;
    private const float DestructionDelay = 0.15f;
    private const float RayLength = 0.2f;
    private float generalTimer;
    private float destructionTimer;
    private bool isObjectSpawned;
    private bool isDeathOrderGiven;


    #endregion

    private void Start()
    {
        if (objectColor != ObjectColor._Empty)
        {
            LevelManager.Instance.IncreaseActiveNonGrayCellCount();
        }
    }

    private void FixedUpdate()
    {
        generalTimer += Time.deltaTime;

        if (generalTimer >= InitialWaitingTimeForSpawn && !isObjectSpawned)
        {
            TrySpawningObject();
        }

        if (generalTimer >= InitialWaitingTimeForDestruction && isObjectSpawned && !isDeathOrderGiven)
        {
            TryDestroySelf();
        }
    }

    private void TryDestroySelf()
    {

        if (!VectorHelper.CheckRaycastUp(RayLength * 2.75f, transform, collisionLayers))
        {
            destructionTimer += Time.deltaTime;
            if (destructionTimer < DestructionDelay)
            {
                return;
            }
            isDeathOrderGiven = true;
            transform.DOScale(Vector3.zero, 1f).onComplete += () =>
            {
                LevelManager.Instance.DecreaseActiveNonGrayCellCount();
                Destroy(gameObject);
            };
        }
    }

    private void TrySpawningObject()
    {
        if (!VectorHelper.CheckRaycastUp(RayLength, transform, collisionLayers))
        {
            var cellObject = Instantiate(objectTypeSO.prefab, objectTargetTransformFromChild.position, Quaternion.Euler(cellObjectSpawnRotation));
            cellObject.GetComponent<Renderer>().sharedMaterial = objectTypeSO.normalMaterials[cellObjectMaterialIndex];
            cellObject.GetComponent<CellObject>().AdjustTransformForSetup();
            cellObject.transform.DOScale(cellObject.transform.localScale, 0.5f).From(Vector3.zero);
            isObjectSpawned = true;
        }
    }

    public void ChangeCellObjectType(ObjectType newObjectType)
    {
        objectType = newObjectType;
    }
}