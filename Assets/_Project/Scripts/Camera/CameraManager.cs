using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camera;

    private void Start()
    {
        PositionCameraIntoTheMiddle();
    }

    private void PositionCameraIntoTheMiddle()
    {
        camera.transform.position = GetProperCameraCoordinates();
    }

    private Vector3 GetProperCameraCoordinates()
    {
        var edgeLength = CellGeneration.Instance.CellHeight - 1f;
        var properCameraEdge = edgeLength / 2;
        return new Vector3(properCameraEdge, properCameraEdge, camera.transform.position.z);
    }
}