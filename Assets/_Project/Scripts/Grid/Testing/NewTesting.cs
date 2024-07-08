using UnityEngine;

public class NewTesting : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;

    private TilemapGrid.TilemapObject.TilemapSprite tilemapSprite;
    private TilemapGrid tilemapGrid;

    private void Start()
    {
        tilemapGrid = new TilemapGrid(20, 10, 5f, Vector3.zero);
        tilemapGrid.SetTilemapVisualGrid(tilemapGrid, tilemapVisual);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            tilemapGrid.SetTilemapSprite(mouseWorldPosition, tilemapSprite);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.None;
            Debug.Log("none");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.Ground;
            Debug.Log("ground");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.Frog;
            Debug.Log("frog");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            tilemapGrid.Save();
            Debug.Log("saved!");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            tilemapGrid.Load();
            Debug.Log("loaded!");
        }
    }
}