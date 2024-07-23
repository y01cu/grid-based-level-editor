using UnityEngine;

public class EditorValueManager : MonoBehaviour
{
    private ColorMaterialsListSO colorMaterialsList;
    public static EditorValueManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        colorMaterialsList = Resources.Load<ColorMaterialsListSO>(nameof(ColorMaterialsListSO));
        Debug.Log($"color materials list: {colorMaterialsList} | length: {colorMaterialsList.colorMaterials.Count}");
    }
}