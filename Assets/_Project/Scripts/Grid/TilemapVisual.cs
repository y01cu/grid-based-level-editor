using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TilemapVisual : MonoBehaviour
{
    [Serializable]
    public struct TilemapSpriteUV
    {
        [FormerlySerializedAs("tilemapSprite")]
        public TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture;

        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private TilemapSpriteUV[] tilemapSpriteUvArray;
    private GridSystem<TilemapGrid.TilemapObject> gridSystem;
    private Mesh mesh;
    private bool isMeshReadyToUpdate;
    private Dictionary<TilemapGrid.TilemapObject.TilemapSpriteTexture, UVCoords> uvCoordsDictionary;
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureHeight = texture.height;
        float textureWidth = texture.width;

        meshRenderer = GetComponent<MeshRenderer>();

        // var meshRenderer = GetComponent<MeshRenderer>();
        // meshRenderer.material.color = Color.cyan;


        uvCoordsDictionary = new Dictionary<TilemapGrid.TilemapObject.TilemapSpriteTexture, UVCoords>();
        foreach (var tilemapSpriteUV in tilemapSpriteUvArray)
        {
            uvCoordsDictionary[tilemapSpriteUV.tilemapSpriteTexture] = new UVCoords
            {
                uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
            };
        }
    }

    public void SetGridSystem(TilemapGrid tilemapGrid, GridSystem<TilemapGrid.TilemapObject> gridSystem)
    {
        this.gridSystem = gridSystem;
        UpdateTilemapVisual();

        gridSystem.OnGridObjectChanged += GridSystemOnGridObjectChanged;
        tilemapGrid.OnLoaded += TilemapGridOnOnLoaded;
    }

    private void TilemapGridOnOnLoaded(object sender, EventArgs e)
    {
        isMeshReadyToUpdate = true;
    }

    private void GridSystemOnGridObjectChanged(object sender, GridSystem<TilemapGrid.TilemapObject>.OnGridValueChangedEventArgs e)
    {
        isMeshReadyToUpdate = true;
    }

    private void LateUpdate()
    {
        if (isMeshReadyToUpdate)
        {
            isMeshReadyToUpdate = false;
            UpdateTilemapVisual();
        }
    }

    private void UpdateTilemapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(gridSystem.Width * gridSystem.Height, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < gridSystem.Width; x++)
        {
            for (int y = 0; y < gridSystem.Height; y++)
            {
                int index = x * gridSystem.Height + y;
                Vector3 quadSize = new Vector3(1, 1) * gridSystem.CellSize;

                TilemapGrid.TilemapObject gridObject = gridSystem.GetGridObjectOnCoordinates(x, y);
                TilemapGrid.TilemapObject.TilemapSpriteTexture tilemapSpriteTexture = gridObject.GetTilemapSprite();

                Vector2 gridValueUV00;
                Vector2 gridValueUV11;
                if (tilemapSpriteTexture == TilemapGrid.TilemapObject.TilemapSpriteTexture.None)
                {
                    gridValueUV00 = Vector2.zero;
                    gridValueUV11 = Vector2.zero;
                    quadSize = Vector3.zero;
                }
                else
                {
                    UVCoords uvCoords = uvCoordsDictionary[tilemapSpriteTexture];
                    gridValueUV00 = uvCoords.uv00;
                    gridValueUV11 = uvCoords.uv11;
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, gridSystem.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV00, gridValueUV11);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}