using System;
using UnityEngine;

public class NewTesting : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;

    private TilemapGrid.TilemapObject.TilemapSprite tilemapSprite;
    private TilemapGrid tilemapGrid;

    private void Start()
    {
        AdjustSpriteButton.AdjustSpriteToColor += SetSprite;
        tilemapGrid = new TilemapGrid(4, 4, 3f, Vector3.zero);
        tilemapGrid.SetTilemapVisualGrid(tilemapGrid, tilemapVisual);
    }

    private void SetSprite(TilemapGrid.TilemapObject.TilemapSprite newTilemapSprite)
    {
        tilemapSprite = newTilemapSprite;
        Debug.Log($"updated to: {newTilemapSprite.ToString()}");
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.Blue;
            Debug.Log("blue");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.Green;
            Debug.Log("green");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.Red;
            Debug.Log("red");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            tilemapSprite = TilemapGrid.TilemapObject.TilemapSprite.Yellow;
            Debug.Log("yellow");
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

    private void OnDestroy()
    {
        AdjustSpriteButton.AdjustSpriteToColor -= SetSprite;
    }
}