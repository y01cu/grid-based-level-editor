using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPositioning : MonoBehaviour
{
    [SerializeField] private Camera camera;

    private void Update()
    {
        var canObjectBePlaced = LevelEditorGridTesting.IsOnGrid && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0);
        if (canObjectBePlaced)
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            PlaceObject(mouseWorldPosition);



        }
    }

    private void PlaceObject(Vector3 mouseWorldPosition)
    {
        LevelEditorGridTesting.Instance.SetupObjectOnPosition(mouseWorldPosition);
        ObjectGhost.Instance.SpawnAndAdjustPrefabOnPosition();
    }
}
