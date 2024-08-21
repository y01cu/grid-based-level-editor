using UnityEngine;

public abstract class CellObject : MonoBehaviour
{
    public ObjectColor objectColor;
    public Material obstacleMaterial;
    public Material normalMaterial;
    public AudioClip obstacleStateClip;
    public ObjectTypeSO objectTypeSO;
    public Vector3 spawnRotation;

    protected Transform indicatorObjectTransform;

    public int materialIndex;

    public bool IsInLevelEditor;

    protected bool isRotating;

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
    public abstract void RotateByAngleInTheEditor(Vector3 angle);

    public abstract void ActivateIndicator();

}