using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Frog : Clickable
{
    public event Action FreeBerriesForFrog;

    // [SerializeField] private LineRenderer lineRenderer;

    // [SerializeField] private LayerMask collisionMask;

    // [SerializeField] private BoxCollider boxCollider;


    private CellGeneration.OrderType orderType;

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private Sequence sequence;

    [SerializeField] private bool isObstacleHit;
    private bool isTongueOutside;
    private bool isTongueReachedEnd;
    private bool isReachedEnd;

    [SerializeField] private float time;
    private float tweenDuration = 1.2f;
    private float interval = 0.15f;
    private float segmentDuration;

    public List<Berry> detectedBerries = new();
    private List<Vector3> points = new();

    private List<GameObject> detectedObjects = new();

    public LineManager lineManager;
    
    public override void OnClickedOverWithTargetScale(Vector3 targetScale)
    {
        if (isTongueOutside)
        {
            return;
        }

        base.OnClickedOverWithTargetScale(targetScale);

        Vector3 startPoint = transform.TransformPoint(lineManager.GetLineRenderer().GetPosition(0));
        Vector3 rotation = transform.parent.localRotation.eulerAngles;
        
        Vector3 direction = (int)rotation.y switch
        {
            0 => Vector3.up,
            90 => Vector3.right,
            180 => Vector3.down,
            270 => Vector3.left,
        };

        Debug.Log("start point: " + startPoint + " | dir: " + direction);

        lineManager.StartProcess(startPoint, direction, orderType, objectColor);
    }

    public void SetOrderType(CellGeneration.OrderType newOrderType)
    {
        orderType = newOrderType;
    }

    public List<Berry> GetDetectedObjects()
    {
        return detectedBerries;
    }

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayAudioClip(obstacleStateClip);

        skinnedMeshRenderer.material = obstacleMaterial;

        await Task.Delay(1000);

        skinnedMeshRenderer.material = normalMaterial;

        // CleanObstacleState();
    }
}