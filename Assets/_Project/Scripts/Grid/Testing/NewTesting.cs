using System;
using UnityEngine;

public class NewTesting : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private TilemapVisual tilemapVisual;

    private TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture;
    private TilemapGrid.TilemapObject.TilemapObjectType tilemapObjectType;
    private TilemapGrid tilemapGrid;

    private string typeName;

    private void Start()
    {
        AdjustTypeButton.AdjustTileObjectType += SetObjectType;
        AdjustSpriteButton.AdjustSpriteTexture += SetSprite;
        tilemapGrid = new TilemapGrid(4, 4, 3f, Vector3.zero);
        tilemapGrid.SetTilemapVisualGrid(tilemapGrid, tilemapVisual);
    }

    private void AdjustTileObject(string newTypeName)
    {
        typeName = newTypeName;
    }

    private void SetSprite(TilemapGrid.TilemapObject.TilemapSpriteTexture newTilemapSpriteTexture)
    {
        tilemapSpriteTexture = newTilemapSpriteTexture;
        Debug.Log($"updated to: {newTilemapSpriteTexture.ToString()}");
    }

    private void SetObjectType(TilemapGrid.TilemapObject.TilemapObjectType newType)
    {
        tilemapObjectType = newType;
        Debug.Log($"changed to type: {tilemapObjectType}");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            tilemapGrid.SetupTilemapOnPosition(mouseWorldPosition, tilemapSpriteTexture, tilemapObjectType);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            tilemapSpriteTexture = TilemapGrid.TilemapObject.TilemapSpriteTexture.None;
            Debug.Log("none");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            tilemapSpriteTexture = TilemapGrid.TilemapObject.TilemapSpriteTexture.Blue;
            Debug.Log("blue");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            tilemapSpriteTexture = TilemapGrid.TilemapObject.TilemapSpriteTexture.Green;
            Debug.Log("green");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            tilemapSpriteTexture = TilemapGrid.TilemapObject.TilemapSpriteTexture.Red;
            Debug.Log("red");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            tilemapSpriteTexture = TilemapGrid.TilemapObject.TilemapSpriteTexture.Yellow;
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
        AdjustSpriteButton.AdjustSpriteTexture -= SetSprite;
    }
}