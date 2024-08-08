using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public void Save()
    {
        LevelEditorManager.tilemapGrid.Save();
        Debug.Log("saved!");
    }

    public void Load()
    {
        LevelEditorManager.tilemapGrid.Load();
        Debug.Log("loaded!");
    }
}
