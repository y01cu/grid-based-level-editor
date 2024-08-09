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

    public void JustCheckCollision(Vector3 startPoint, Vector3 direction, string frogColor)
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
            var hitObjSO = firstHit.GetComponent<CellObject>().objectTypeSO;
            bool isDifferentColor = hitObjSO.normalMaterials[hitObjSO.materialIndex].name != frogColor;
            Debug.Log($"names: {hitObjSO.normalMaterials[hitObjSO.materialIndex].name} and {frogColor}");

            if (isDifferentColor)
            {
                Debug.Log("hit something with a different color");
                Vector3 obstaclePoint;

                obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.transform.localPosition);
                // if (firstHit.transform.parent != null)
                // {
                // }
                // else
                // {
                //     obstaclePoint = transform.parent.InverseTransformPoint(firstHit.transform.localPosition);
                // }

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
                HandleArrowCollision(currentCollider, frogColor);

                isAnyArrowHit = true;
            }

            if (currentCollider.CompareTag("Berry"))
            {
                HandleBerryCollision(currentCollider, frogColor);
            }
        }

        if (hits.Length != 0 && !isAnyArrowHit && !isCollidedWithLastBerry && !IsObstacleHit)
        {
            JustCheckCollision(startPoint + direction, direction, frogColor);
        }
    }

    private void HandleArrowCollision(Collider currentCollider, string frogColor)
    {
        Direction arrowDirection = currentCollider.GetComponent<Arrow>().GetDirection();

        Vector3 newPoint = transform.parent.InverseTransformPoint(currentCollider.transform.localPosition);
        detectedObjectStorage.points.Add(newPoint);

        // Since arrows change direction and the movement continues check collision till finding last berry. 
        JustCheckCollision(currentCollider.transform.position, VectorHelper.GetDirectionVector(arrowDirection), frogColor);
    }

    private void HandleBerryCollision(Collider currentCollider, string frogColor)
    {
        Berry berry = currentCollider.GetComponent<Berry>();
        var isBerryPickable = berry.GetComponent<Renderer>().sharedMaterial.name == frogColor && !berry.IsDetected();
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