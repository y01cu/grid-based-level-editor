using System;
using System.Linq;
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
        Destroy(hits[hits.Length - 1].collider.gameObject);
        var gridSystem = LevelEditorManager.tilemapGrid.gridSystem;
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)?.DeleteLastTilemapObject();
        OnEndedRemovingObject?.Invoke(this, EventArgs.Empty);
    }

    private void TryPlacingObject(Vector3 mouseWorldPosition, Vector3 objectRotation)
    {
        if (ObjectGhost.Instance.objectTypeSO == null || ObjectGhost.Instance.IsObjectReadyToBePlaced == false)
        {
            return;
        }
        LevelEditorManager.Instance.SetupObjectOnPosition(mouseWorldPosition, objectRotation);
        ObjectGhost.Instance.SpawnAndAdjustPrefabOnPosition(objectRotation);
    }
}
