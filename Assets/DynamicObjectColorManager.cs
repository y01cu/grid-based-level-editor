using UnityEngine;

public class DynamicObjectColorManager : MonoBehaviour
{
    private ObjectTypeSO activeObjectTypeSO;

    private void Start()
    {
        AdjustTypeButton.OnActiveObjectUpdated += AdjustTypeButtonOnActiveObjectUpdated;
    }

    private void AdjustTypeButtonOnActiveObjectUpdated(object sender, OnActiveObjectTypeChangedEventArgs e)
    {
        activeObjectTypeSO = e.activeObjectTypeSO;
        ShowColorsOfCurrentObjectType();
    }

    private void ShowColorsOfCurrentObjectType()
    {
        Debug.Log($"{activeObjectTypeSO.name} has this colors:");
        foreach (Material material in activeObjectTypeSO.normalMaterials)
        {
            Debug.Log($"<>{material.name}", material);
        }
    }
}