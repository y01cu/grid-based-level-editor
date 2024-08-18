using UnityEngine;
using UnityEngine.UI;

public class LevelButton : BasicButton
{
    public void UpdateLevel()
    {
        int levelIndex = transform.GetSiblingIndex() + 1;
        Debug.Log("Level index: " + levelIndex);
        LevelEditorManager.Instance.LevelIndex = levelIndex;
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
