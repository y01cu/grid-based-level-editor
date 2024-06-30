using UnityEngine;

public abstract class CellObject : MonoBehaviour
{
    public ObjectColor objectColor;
    public ObjectType objectType;
    
    public Material obstacleMaterial;
    public Material normalMaterial;

    public AudioClip obstacleStateClip;

    public abstract void HandleBeingObstacle();
}