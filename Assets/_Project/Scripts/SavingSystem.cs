using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public void Save()
    {
        LevelEditorGridTesting.tilemapGrid.Save();
        Debug.Log("saved!");
    }

    public void Load()
    {
        LevelEditorGridTesting.tilemapGrid.Load();
        Debug.Log("loaded!");
    }
}
