using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Frog : Clickable
{
    public LineManager lineManager;

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private OrderType orderType;

    private WaitForSeconds initialDelayForClick = new(2f);

    protected IEnumerator Start()
    {
        isTweenable = false;
        yield return initialDelayForClick;
        isTweenable = true;
    }

    public override void OnClickedOverWithTargetScale(Vector3 targetScale)
    {
        // Disable the click event if the tongue is outside
        if (lineManager.IsLineOutside)
        {
            return;
        }

        base.OnClickedOverWithTargetScale(targetScale);

        if (IsInLevelEditor)
        {
            return;
        }

        Vector3 startPoint = transform.TransformPoint(lineManager.GetLineRenderer().GetPosition(0));
        Vector3 rotation = transform.localRotation.eulerAngles;

        Vector3 direction = RoundToNearestMultipleOf90(rotation.y) switch
        {
            0 => Vector3.up,
            90 => Vector3.right,
            180 => Vector3.down,
            270 => Vector3.left,
            _ => Vector3.up,
        };
        Debug.Log("started moving tongue line");
        lineManager.MoveTongueLine(startPoint, direction, GetComponent<Renderer>().sharedMaterial.name);
    }

    private int RoundToNearestMultipleOf90(float angle)
    {
        return Mathf.RoundToInt(angle / 90) * 90;
    }

    public void SetOrderType(OrderType newOrderType)
    {
        orderType = newOrderType;
    }

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayCustomAudioClip(obstacleStateClip);

        skinnedMeshRenderer.material = obstacleMaterial;

        await Task.Delay(1000);

        skinnedMeshRenderer.material = normalMaterial;
    }

    public override void AdjustTransformForSetup()
    {
    }

    public override void RotateByAngleInTheEditor(Vector3 angle)
    {
        if (isRotating == false)
        {
            isRotating = true;

            Quaternion finalRotation = transform.rotation * Quaternion.Euler(angle);
            transform.DORotate(finalRotation.eulerAngles, 0.2f).onComplete += () => isRotating = false;
        }
    }
}