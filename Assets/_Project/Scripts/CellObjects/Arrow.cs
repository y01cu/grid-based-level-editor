using System.Threading.Tasks;
using UnityEngine;

public class Arrow : CellObject
{
    private Direction direction;

    [SerializeField] private MeshRenderer meshRenderer;

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayAudioClip(obstacleStateClip);

        meshRenderer.material = obstacleMaterial;

        await Task.Delay(500);

        meshRenderer.material = normalMaterial;
    }

    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
    }

    public Direction GetDirection()
    {
        return direction;
    }
}