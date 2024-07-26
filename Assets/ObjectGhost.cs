using UnityEngine;

public class ObjectGhost : MonoBehaviour
{
    public static ObjectGhost Instance { get; private set; }


    [SerializeField] private Camera camera;

    private GameObject spriteGameObject;

    public GameObject prefab;
    private Vector3 tempPosition;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverAudioClip;

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
        tempPosition = spriteGameObject.transform.position;

        AdjustTypeButton.OnActiveBuildingTypeChanged += AdjustTypeButton_OnActiveBuildingTypeChanged;
    }

    private void Update()
    {
        float cellSize = LevelEditorGridTesting.Instance.cellSize;
        spriteGameObject.transform.position = LevelEditorGridTesting.tilemapGrid.gridSystem.GetGridPosition(camera.ScreenToWorldPoint(Input.mousePosition)).vector3With0Z * cellSize + new Vector3(cellSize / 2, cellSize / 2, 0);
        if (tempPosition == spriteGameObject.transform.position)
        {
            Debug.Log("same");
        }
        else
        {
            // Debug.Log("not same");
            audioSource.PlayOneShot(hoverAudioClip);
        }

        tempPosition = spriteGameObject.transform.position;
    }

    public void SpawnAndAdjustPrefabOnPosition()
    {
        var spawnedPrefab = Instantiate(prefab, spriteGameObject.transform.position, Quaternion.Euler(90, 0, 0));
        spawnedPrefab.transform.Translate(0, -2f, 0);
        spawnedPrefab.transform.localScale *= LevelEditorGridTesting.Instance.cellSize;
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
    }
}