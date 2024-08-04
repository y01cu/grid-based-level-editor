using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPositioning : MonoBehaviour
{
    [SerializeField] private Camera camera;

    public static event EventHandler OnRemovingObjectStarted;
    public static event EventHandler OnRemovingObjectEnded;

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
        var canObjectBePlaced = LevelEditorGridTesting.IsOnGrid && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0);
        if (canObjectBePlaced)
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 objectRotation = ObjectGhost.Instance.GetCurrentObjectRotation();
            PlaceObject(mouseWorldPosition, objectRotation);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            RemoveObject();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetObjectMaterial();
        }
    }

    public Material GetObjectMaterial()
    {

        var hits = VectorHelper.GetRaycastHitsFromMousePosition(camera);

        // Create a HashSet to store unique object names
        HashSet<string> uniqueObjectNames = new HashSet<string>();

        for (int i = 0; i < hits.Length; i++)
        {
            string objectName = hits[i].collider.gameObject.name;

            // Check if the object name is already in the HashSet
            if (!uniqueObjectNames.Contains(objectName))
            {
                uniqueObjectNames.Add(objectName);
                // Debug.Log($"index: {i} {objectName}");
            }
        }

        return hits[hits.Length - 1].collider.gameObject.GetComponent<Renderer>().material;
    }

    private void RemoveObject()
    {
        OnRemovingObjectStarted?.Invoke(this, EventArgs.Empty);

        var hits = VectorHelper.GetRaycastHitsFromMousePosition(camera);
        // Create a HashSet to store unique object names
        HashSet<string> uniqueObjectNames = new HashSet<string>();

        for (int i = 0; i < hits.Length; i++)
        {
            string objectName = hits[i].collider.gameObject.name;

            // Check if the object name is already in the HashSet
            if (!uniqueObjectNames.Contains(objectName))
            {
                uniqueObjectNames.Add(objectName);
                Debug.Log($"index: {i} {objectName}");
            }
        }

        OnRemovingObjectEnded?.Invoke(this, EventArgs.Empty);
        Destroy(hits[hits.Length - 1].collider.gameObject);

        var gridSystem = LevelEditorGridTesting.tilemapGrid.gridSystem;
        Vector3 cameraToWorldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        TilemapGrid.TilemapObject tilemapObject = gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint);
        var objectRotation = ObjectGhost.Instance.GetCurrentObjectRotation();
        gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)?.UpdateTilemapObject(tilemapObject.GetObjectTypeSO().materialIndex, null, objectRotation);
    }


    private void PlaceObject(Vector3 mouseWorldPosition, Vector3 objectRotation)
    {
        LevelEditorGridTesting.Instance.SetupObjectOnPosition(mouseWorldPosition, objectRotation);
        ObjectGhost.Instance.SpawnAndAdjustPrefabOnPosition();
    }
}
