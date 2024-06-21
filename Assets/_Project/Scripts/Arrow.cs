using System;
using System.Threading.Tasks;
using UnityEngine;

public class Arrow : CellObject
{
    public Direction direction;

    [SerializeField] private MeshRenderer meshRenderer;

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayAudioClip(obstacleStateClip);

        meshRenderer.material = obstacleMaterial;

        await Task.Delay(1000);

        meshRenderer.material = normalMaterial;

        CleanObstacleState();
    }
}