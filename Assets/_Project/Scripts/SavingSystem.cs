using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public void Save()
    {
        LevelEditorManager.tilemapGrid.SaveLevelWithIndex(LevelEditorManager.Instance.LevelIndex);
        Debug.Log("saved!");
    }

    public void Load()
    {
        LevelEditorManager.tilemapGrid.LoadForEditor();
        Debug.Log("loaded!");
    }
}
