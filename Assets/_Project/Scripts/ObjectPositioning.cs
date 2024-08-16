using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPositioning : MonoBehaviour
{
    [SerializeField] private Camera camera;

    public static event EventHandler OnStartedRemovingObject;
    public static event EventHandler OnEndedRemovingObject;

    public static ObjectPositioning Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        var canObjectBePlaced = LevelEditorManager.IsOnGrid && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0);
        if (canObjectBePlaced)
        {
            TryPlacingObject(camera.ScreenToWorldPoint(Input.mousePosition), ObjectGhost.Instance.GetCurrentObjectRotation());
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RemoveObject();
        }
    }

    private void RemoveObject()
    {
        OnStartedRemovingObject?.Invoke(this, EventArgs.Empty);
        var hits = VectorHelper.GetRaycastHitsFromMousePosition(camera);
        Destroy(GetCellObjectFromHits(hits));
        var gridSystem = LevelEditorManager.tilemapGrid.gridSystem;
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)?.DeleteLastTilemapObject();
        OnEndedRemovingObject?.Invoke(this, EventArgs.Empty);
    }

    private GameObject GetCellObjectFromHits(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.GetComponent<CellObject>() != null)
            {
                return hits[i].collider.gameObject;
            }
        }
        return null;
    }

    private void DeactivateCellObjectsOfCellBases(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            var currentCellBase = hits[i].collider.gameObject.GetComponent<CellBase>();
            if (currentCellBase != null)
            {
                currentCellBase.ResetBackToInitialState();
            }
        }

    }

    private void TryPlacingObject(Vector3 mouseWorldPosition, Vector3 objectRotation)
    {
        if (ObjectGhost.Instance.objectTypeSO == null || ObjectGhost.Instance.IsObjectReadyToBePlaced == false)
        {
            return;
        }
        DeactivateCellObjectsOfCellBases(VectorHelper.GetRaycastHitsFromMousePosition(camera));
        LevelEditorManager.Instance.SetupObjectOnPosition(mouseWorldPosition, objectRotation);
    }
}
