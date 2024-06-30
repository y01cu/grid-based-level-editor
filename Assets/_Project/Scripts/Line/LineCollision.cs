using JetBrains.Annotations;
using UnityEngine;

public class LineCollision : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;

    [NotNull] public DetectedObjectStorage detectedObjectStorage = new();

    private bool isCollidedWithLastBerry;
    private bool isObstacleHit;

    public bool IsObstacleHit
    {
        get => isObstacleHit;
        set => isObstacleHit = value;
    }

    public bool JustCheckCollisionReturnIfHit(Vector3 startPoint, Vector3 direction, CellGeneration.OrderType orderType, CellBase.ObjectColor objectColor)
    {
        Debug.Log($"start point: {startPoint}");

        Debug.DrawRay(startPoint, direction, Color.black, 3f);
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        if (hits.Length == 0)
        {
            return IsObstacleHit;
        }

        Transform firstHit = hits[0].transform;

        if (hits.Length > 0)
        {
            bool isDifferentColor = firstHit.transform.GetComponent<CellObject>().objectColor != objectColor;

            Debug.Log($"is color dif: {isDifferentColor}", firstHit);

            if (isDifferentColor)
            {
                Debug.Log("different color collision");
                Debug.Log($"hit obs: {firstHit.transform.gameObject.name} | total point count: {detectedObjectStorage.points.Count}", firstHit);
                Vector3 obstaclePoint;

                if (firstHit.transform.parent != null)
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.parent.transform.localPosition);
                }
                else
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.localPosition);
                }

                // Debug.Log("btw added point: " + obstaclePoint);

                detectedObjectStorage.detectedObjects.Add(firstHit.gameObject);
                detectedObjectStorage.points.Add(obstaclePoint);
                Debug.Log($"added point l59 {obstaclePoint}");

                Debug.Log("returning true");

                IsObstacleHit = true;

                return IsObstacleHit;
            }

            // if (firstHit.transform.GetComponent<CellObject>().objectType == CellBase.ObjectType.Frog)
            // {
            //     Debug.Log("frog true");
            //     
            //     return true;
            // }
        }

        Debug.Log($"total point count: {detectedObjectStorage.points.Count}");

        bool isAnyArrowHit = false;

        foreach (var hit in hits)
        {
            Collider currentCollider = hit.collider;

            if (currentCollider.CompareTag("Arrow"))
            {
                Debug.Log("collided with arrow");
                HandleArrowCollision(currentCollider, orderType, objectColor);
                // isAnyArrowHit = LongArrowCheck(orderType, objectColor, currentCollider);
                isAnyArrowHit = true;
            }

            if (currentCollider.CompareTag("Berry"))
            {
                HandleBerryCollision(currentCollider, objectColor);
            }
        }

        Debug.Log("checking obs hit here again");

        if (hits.Length != 0 && !isAnyArrowHit && !isCollidedWithLastBerry && !IsObstacleHit)
        {
            JustCheckCollisionReturnIfHit(startPoint + direction, direction, orderType, objectColor);
        }

        return IsObstacleHit;
    }

    private void HandleArrowCollision(Collider currentCollider, CellGeneration.OrderType orderType, CellBase.ObjectColor objectColor)
    {
        Direction arrowDirection = currentCollider.GetComponent<Arrow>().GetDirection();

        Vector3 newPoint = transform.parent.InverseTransformPoint(currentCollider.transform.localPosition);
        detectedObjectStorage.points.Add(newPoint);
        Debug.Log($"added point l116 {newPoint}");

        // Since arrows change direction and the movement continues check collision till finding last berry. 
        JustCheckCollisionReturnIfHit(currentCollider.transform.position, VectorHelper.GetDirectionVector(arrowDirection), orderType, objectColor);
    }

    private void HandleBerryCollision(Collider currentCollider, CellBase.ObjectColor objectColor)
    {
        Berry berry = currentCollider.GetComponent<Berry>();

        var isBerryPickable = berry.objectColor == objectColor && !berry.IsDetected();

        if (isBerryPickable)
        {
            berry.SetAsDetected();
            detectedObjectStorage.detectedObjects.Add(berry.gameObject);
            detectedObjectStorage.detectedBerries.Add(berry);
        }

        if (berry.IsLastBerryForFrog())
        {
            isCollidedWithLastBerry = true;
        }
    }
}