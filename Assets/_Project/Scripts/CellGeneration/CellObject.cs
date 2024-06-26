using UnityEngine;

public abstract class CellObject : MonoBehaviour
{
    public CellBase.ObjectColor objectColor;
    public CellBase.ObjectType objectType;
    
    public Material obstacleMaterial;
    public Material normalMaterial;

    public AudioClip obstacleStateClip;

    public abstract void HandleBeingObstacle();
}