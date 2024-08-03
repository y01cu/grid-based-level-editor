using UnityEngine;
using UnityEngine.Serialization;

public abstract class CellObject : MonoBehaviour
{
    public ObjectColor objectColor;

    // public ObjectType objectType;
    public Material obstacleMaterial;
    public Material normalMaterial;
    public AudioClip obstacleStateClip;

    public ObjectTypeSO objectTypeSO;

    public abstract void HandleBeingObstacle();

    public virtual void Initialize(ObjectColor objectColor, ObjectType objectType)
    {
        this.objectColor = objectColor;
        // this.objectType = objectType;
    }

    /// <summary>
    /// Includes adjusting position, rotation and scale of the object.
    /// </summary>
    public virtual void AdjustTransform()
    {
        AdjustPosition();
        AdjustRotation();
        AdjustScale();
    }

    protected virtual void AdjustPosition()
    {
    }

    protected virtual void AdjustRotation()
    {

    }
    protected virtual void AdjustScale()
    {

    }

}