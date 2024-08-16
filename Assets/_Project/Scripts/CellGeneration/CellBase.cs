using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
    public bool isInLevelEditor;
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
    public bool isLoading;

    #endregion

    private void Start()
    {
        if (objectColor != ObjectColor._Empty)
        {
            LevelManager.Instance?.IncreaseActiveNonGrayCellCount();
        }
    }

    private void FixedUpdate()
    {
        generalTimer += Time.deltaTime;

        if (generalTimer >= InitialWaitingTimeForSpawn && !isObjectSpawned && !VectorHelper.CheckRaycastUp(RayLength * 10f, transform, collisionLayers))
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
        if (!VectorHelper.CheckRaycastUp(RayLength * 10f, transform, collisionLayers))
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

        return;
    }

    private void TrySpawningObject()
    {
        if (!VectorHelper.CheckRaycastUp(RayLength, transform, collisionLayers))
        {
            isObjectSpawned = true;
            var spawnedObject = Instantiate(objectTypeSO.prefab, objectTargetTransformFromChild.position, Quaternion.Euler(cellObjectSpawnRotation));
            cellObject = spawnedObject.GetComponent<CellObject>();

            if (isInLevelEditor)
            {
                cellObject.IsInLevelEditor = true;
                if (!isLoading)
                {
                    spawnedObject.transform.Rotate(spawnedObject.GetComponent<CellObject>().spawnRotation);
                }
            }
            spawnedObject.GetComponent<Renderer>().sharedMaterial = objectTypeSO.normalMaterials[cellObjectMaterialIndex];

            if (LevelEditorManager.Instance != null)
            {
                spawnedObject.transform.localScale *= LevelEditorManager.Instance.cellSize;
            }

            spawnedObject.transform.DOScale(spawnedObject.transform.localScale, 0.5f).From(Vector3.zero).onComplete += () =>
            {
                cellObject.AdjustTransformForSetup();
            };
        }
    }

    public void ChangeCellObjectType(ObjectType newObjectType)
    {
        objectType = newObjectType;
    }

    public void ResetBackToInitialState()
    {
        if (cellObject != null)
        {
            Destroy(cellObject.gameObject);
        }
        generalTimer = 0;
        destructionTimer = 0;
        isObjectSpawned = false;
        isDeathOrderGiven = false;
    }
}