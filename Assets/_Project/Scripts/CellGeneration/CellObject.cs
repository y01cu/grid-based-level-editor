using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CellObject : MonoBehaviour
{
    public ObjectColor objectColor;
    public Material obstacleMaterial;
    public Material normalMaterial;
    public AudioClip obstacleStateClip;
    public ObjectTypeSO objectTypeSO;

    public abstract void HandleBeingObstacle();

    public virtual void Initialize(ObjectColor objectColor)
    {
        this.objectColor = objectColor;
    }

    /// <summary>
    /// Includes adjusting position, rotation and scale of the object.
    /// </summary>
    public abstract void AdjustTransformForSetup();

    /// <summary>
    /// Rotate certain cell objects in the grid level editor.
    /// </summary>
    /// <param name="angle"></param>
    public abstract void RotateByAngle(Vector3 angle);

}