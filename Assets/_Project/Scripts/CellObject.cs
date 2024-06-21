using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CellObject : MonoBehaviour
{
    public Material obstacleMaterial;
    public Material normalMaterial;

    public AudioClip obstacleStateClip;

    protected bool isObstacle;

    public void InspectObstacleStateTongueHit()
    {
        if (isObstacle)
        {
            HandleBeingObstacle();
        }
    }

    public void SetAsObstacle()
    {
        isObstacle = true;
    }

    public void CleanObstacleState()
    {
        isObstacle = false;
    }

    public abstract void HandleBeingObstacle();
    // {
    // }
    // objectMeshRenderer.material = obstacleMaterial;
    //
    // AudioManager.Instance.PlayAudioClip(obstacleStateClip);
    //
    // await Task.Delay(1000);
    //
    // objectMeshRenderer.material = normalMaterial;
}