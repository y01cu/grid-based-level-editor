using System.Collections;
using System.Threading.Tasks;
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
        if (lineManager.IsLineOutside)
        {
            return;
        }

        base.OnClickedOverWithTargetScale(targetScale);

        Vector3 startPoint = transform.TransformPoint(lineManager.GetLineRenderer().GetPosition(0));
        Vector3 rotation = transform.parent.localRotation.eulerAngles;

        Vector3 direction = (int)rotation.y switch
        {
            0 => Vector3.up,
            90 => Vector3.right,
            180 => Vector3.down,
            270 => Vector3.left,
        };

        lineManager.MoveTongueLine(startPoint, direction, orderType, objectColor);
    }

    public void SetOrderType(OrderType newOrderType)
    {
        orderType = newOrderType;
    }

    public override async void HandleBeingObstacle()
    {
        AudioManager.Instance.PlayAudioClip(obstacleStateClip);

        skinnedMeshRenderer.material = obstacleMaterial;

        await Task.Delay(1000);

        skinnedMeshRenderer.material = normalMaterial;
    }

    public override void AdjustTransform()
    {
        AdjustPosition();
        AdjustRotation();
        AdjustScale();
    }
    protected override void AdjustPosition()
    {
        transform.Translate(0, 0.5f, 0);
    }

    protected override void AdjustRotation()
    {

    }
    protected override void AdjustScale()
    {

    }
}