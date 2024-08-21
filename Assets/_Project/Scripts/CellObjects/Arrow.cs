using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using QFSW.QC;
using UnityEngine;

public class Arrow : CellObject
{
    [SerializeField] private MeshRenderer meshRenderer;

    private Direction currentDirection;
    public Direction CurrentDirection { get => currentDirection; set => currentDirection = value; }

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayCustomAudioClip(obstacleStateClip);

        meshRenderer.material = obstacleMaterial;

        await Task.Delay(500);

        meshRenderer.material = normalMaterial;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        AssignNormalMaterial();
    }

    private void AssignNormalMaterial()
    {
        normalMaterial = meshRenderer.material;
    }

    public override void AdjustTransformForSetup()
    {
        int moddedRotationValue = (Convert.ToInt32(transform.rotation.eulerAngles.z) + 360) % 360;
        Direction direction = moddedRotationValue switch
        {
            0 => Direction.Left,
            90 => Direction.Down,
            180 => Direction.Right,
            270 => Direction.Up
        };
        CurrentDirection = direction;
    }

    public override void RotateByAngleInTheEditor(Vector3 angle)
    {
        if (isRotating == false)
        {
            isRotating = true;
            var properAngleForArrow = new Vector3(0, 0, angle.y);
            transform.DORotate(transform.rotation.eulerAngles + properAngleForArrow, 0.2f).onComplete += () => isRotating = false;
        }
    }

    public override void ActivateIndicator()
    {
        indicatorObjectTransform = transform.Find("Cylinder");
        indicatorObjectTransform.gameObject.SetActive(true);
    }
}