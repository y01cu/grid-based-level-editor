using System;
using DG.Tweening;
using UnityEngine;

public class CellBase : MonoBehaviour
{
    public GameObject[] CellObjectPrefabs;
    public ObjectColor objectColor;
    public ObjectType objectType;
    public Direction arrowDirection;
    public Vector3 additionalRotationVector3;
    public OrderType orderType;

    [SerializeField] private Transform objectTargetTransformFromChild;
    [SerializeField] private LayerMask collisionLayers;

    private const float InitialWaitingTime = 2f;
    private const float DestructionWaitingTime = 3.5f;
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
        if (objectColor == ObjectColor._Empty)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= InitialWaitingTime && !isObjectSpawned)
        {
            TrySpawnObject();
        }

        if (timer >= DestructionWaitingTime && isObjectSpawned && !isDeathOrderGiven)
        {
            TryDestroyObject();
        }
    }

    private void TryDestroyObject()
    {
        if (!RaycastUp(RayLength * 2.75f))
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
        if (!RaycastUp(RayLength))
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

    private bool RaycastUp(float length)
    {
        return Physics.Raycast(transform.position, transform.up, out RaycastHit hitInfo, length, collisionLayers);
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
}