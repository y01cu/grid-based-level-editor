using Unity.Mathematics;
using UnityEngine;

public class ObjectGhost : MonoBehaviour
{
    public static ObjectGhost Instance { get; private set; }


    [SerializeField] private Camera camera;

    private GameObject spriteGameObject;

    public GameObject prefab;

    private void Awake()
    {
        Instance = this;
        spriteGameObject = transform.Find("Sprite").gameObject;
        Hide();
    }

    private void AdjustTypeButton_OnActiveBuildingTypeChanged(object sender, OnActiveBuildingTypeChangedEventArgs e)
    {
        if (e.activeObjectTypeSO == null)
        {
            Hide();
        }
        else
        {
            SetSprite(e.activeObjectTypeSO.spriteForLevelEditor);
            SetPrefab(e.activeObjectTypeSO.prefab.gameObject);
        }
    }

    private void Start()
    {
        AdjustTypeButton.OnActiveBuildingTypeChanged += AdjustTypeButton_OnActiveBuildingTypeChanged;
    }

    private void Update()
    {
        float cellSize = NewTesting.Instance.cellSize;
        spriteGameObject.transform.position = NewTesting.tilemapGrid.gridSystem.GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0);
    }

    public void SpawnPrefabOnPosition()
    {
        Instantiate(prefab, spriteGameObject.transform.position, Quaternion.Euler(90, 0, 0)).transform.Translate(0, -2f, 0);
        Debug.Log("spawned the object in this line of code");
    }

    private void Hide()
    {
        spriteGameObject.SetActive(false);
    }

    private void Show()
    {
        spriteGameObject.SetActive(true);
    }


    private void SetSprite(Sprite newSprite)
    {
        spriteGameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        Show();
    }

    private void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
        // var spawnedPrefab = Instantiate(newPrefab, spriteGameObject.transform.position, Quaternion.identity);
        // spawnedPrefab.transform.SetParent(spriteGameObject.transform);
        // spawnedPrefab.transform.Translate(0, 0, -3f);
        // Debug.Log("here's this one", spawnedPrefab);
    }
}