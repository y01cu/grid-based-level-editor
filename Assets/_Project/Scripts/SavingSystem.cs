using System;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public static void Save()
    {
        LevelEditorManager.tilemapGrid.SaveLevelWithIndex(LevelEditorManager.Instance.LevelIndex);
        Debug.Log("saved!");
    }

    public static void Load()
    {
        LevelEditorManager.tilemapGrid.LoadForEditor();
        Debug.Log("loaded!");
    }

    public static void SaveTemplate()
    {
        LevelEditorManager.tilemapGrid.SaveLevelWithIndex(-1);
    }
}
