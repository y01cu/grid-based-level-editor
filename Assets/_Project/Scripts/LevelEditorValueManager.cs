using TMPro;
using UnityEngine;

public class LevelEditorValueManager : MonoBehaviour
{
    private ColorMaterialsListSO colorMaterialsList;
    public static LevelEditorValueManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        colorMaterialsList = Resources.Load<ColorMaterialsListSO>(nameof(ColorMaterialsListSO));
    }
}