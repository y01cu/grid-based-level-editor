using UnityEngine;

public class LineCollision : MonoBehaviour
{
    public DetectedObjectStorage detectedObjectStorage = new();
    [SerializeField] private LayerMask collisionMask;

    private bool isCollidedWithLastBerry;
    private bool isObstacleHit;

    public bool IsObstacleHit
    {
        get => isObstacleHit;
        set => isObstacleHit = value;
    }

    public void JustCheckCollision(Vector3 startPoint, Vector3 direction, OrderType orderType, ObjectColor objectColor)
    {
        Debug.DrawRay(startPoint, direction, Color.black, 3f);
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, collisionMask);

        if (hits.Length == 0)
        {
            return;
        }

        Transform firstHit = hits[0].transform;

        if (hits.Length > 0)
        {
            bool isDifferentColor = firstHit.transform.GetComponent<CellObject>().objectColor != objectColor;

            if (isDifferentColor)
            {
                Vector3 obstaclePoint;

                if (firstHit.transform.parent != null)
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.parent.transform.localPosition);
                }
                else
                {
                    obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.localPosition);
                }

                detectedObjectStorage.detectedObjects.Add(firstHit.gameObject);
                detectedObjectStorage.points.Add(obstaclePoint);

                IsObstacleHit = true;

                return;
            }
        }

        bool isAnyArrowHit = false;

        foreach (var hit in hits)
        {
            Collider currentCollider = hit.collider;

            if (currentCollider.CompareTag("Arrow"))
            {
                HandleArrowCollision(currentCollider, orderType, objectColor);

                isAnyArrowHit = true;
            }

            if (currentCollider.CompareTag("Berry"))
            {
                HandleBerryCollision(currentCollider, objectColor);
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit && !isCollidedWithLastBerry && !IsObstacleHit)
        {
            JustCheckCollision(startPoint + direction, direction, orderType, objectColor);
        }
    }

    private void HandleArrowCollision(Collider currentCollider, OrderType orderType, ObjectColor objectColor)
    {
        Direction arrowDirection = currentCollider.GetComponent<Arrow>().GetDirection();

        Vector3 newPoint = transform.parent.InverseTransformPoint(currentCollider.transform.localPosition);
        detectedObjectStorage.points.Add(newPoint);

        // Since arrows change direction and the movement continues check collision till finding last berry. 
        JustCheckCollision(currentCollider.transform.position, VectorHelper.GetDirectionVector(arrowDirection), orderType, objectColor);
    }

    private void HandleBerryCollision(Collider currentCollider, ObjectColor objectColor)
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