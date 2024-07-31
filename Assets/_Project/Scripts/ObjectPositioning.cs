using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPositioning : MonoBehaviour
{
    [SerializeField] private Camera camera;

    public static event EventHandler OnRemovingObjectStarted;
    public static event EventHandler OnRemovingObjectEnded;

    private void Update()
    {
        var canObjectBePlaced = LevelEditorGridTesting.IsOnGrid && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0);
        if (canObjectBePlaced)
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            PlaceObject(mouseWorldPosition);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnRemovingObjectStarted?.Invoke(this, EventArgs.Empty);
            // Fire a ray and detect all objects at the mouse position from top to bottom
            RaycastHit[] hits;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray);

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
            gridSystem.GetGridObjectOnCoordinates(cameraToWorldPoint)?.SetTilemapSpriteAndSO(TilemapGrid.TilemapObject.TilemapSpriteTexture.None, null);
        }

        // if (Input.GetMouseButtonDown(1))
        // {
        //     // Fire a ray and detect all objects at the mouse position from top to bottom
        //     RaycastHit[] hits;
        //     Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //     hits = Physics.RaycastAll(ray);

        //     for (int i = 0; i < hits.Length; i++)
        //     {
        //         Debug.Log($"index: {i} {hits[i].collider.gameObject.name}");
        //     }

        // }

        // ---

        // LevelEditorGridTesting.tilemapGrid.gridSystem.SetGridObject
    }

    private void PlaceObject(Vector3 mouseWorldPosition)
    {
        LevelEditorGridTesting.Instance.SetupObjectOnPosition(mouseWorldPosition);
        ObjectGhost.Instance.SpawnAndAdjustPrefabOnPosition();

        // ---


    }
}
