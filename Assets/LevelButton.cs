using UnityEngine;
using UnityEngine.UI;

public class LevelButton : BasicButton
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            UpdateLevel();
            UpdateOutLine();
        });
    }

    public void UpdateLevel()
    {
        int levelIndex = transform.GetSiblingIndex() + 1;
        Debug.Log("Level index: " + levelIndex);
        LevelEditorManager.Instance.LevelIndex = levelIndex;
        LevelEditorManager.Instance.activeLevelText.text = $"LEVEL {levelIndex}";
        SavingSystem.Load();
    }

    public void UpdateOutLine()
    {
        foreach (Transform buttonTransform in transform.parent)
        {
            var outline = buttonTransform.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
        GetComponent<Outline>().enabled = true;
    }
}
