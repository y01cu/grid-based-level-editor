using UnityEngine;

public abstract class CellObject : MonoBehaviour
{
    public ObjectColor objectColor;
    public ObjectType objectType;
    public Material obstacleMaterial;
    public Material normalMaterial;
    public AudioClip obstacleStateClip;

    public abstract void HandleBeingObstacle();

    public virtual void Initialize(ObjectColor objectColor, ObjectType objectType)
    {
        this.objectColor = objectColor;
        this.objectType = objectType;
    }
}