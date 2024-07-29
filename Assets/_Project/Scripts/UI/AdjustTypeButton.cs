using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustTypeButton : MonoBehaviour
{
    public static event EventHandler<OnActiveObjectTypeChangedEventArgs> OnActiveObjectUpdated;

    [SerializeField] private ObjectTypeSO objectTypeSO;
    [SerializeField] private Button colorButtonTemplate;
    [SerializeField] private Transform targetTransformParent;
    private Vector2 anchoredPosition;
    private List<Button> spawnedButtons = new();

    private const float GapBetweenButtons = 60f;

    private void Start()
    {
        AssignColorUsingSpecificPixelOfTexture();
    }

    public void UpdateObjectTypeFromButtonSO()
    {
        ShowObjectMaterialColorButtons();
        OnActiveObjectUpdated?.Invoke(this, new OnActiveObjectTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
    }

    private void ShowObjectMaterialColorButtons()
    {
        var colorMaterialCountOfCurrentObject = objectTypeSO.normalMaterials.Count;
        anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < colorMaterialCountOfCurrentObject; i++)
        {
            var newColorButton = Instantiate(colorButtonTemplate);
            spawnedButtons.Add(newColorButton);
            newColorButton.gameObject.SetActive(true);
            SetButtonPosition(newColorButton, i);
            AssignFunctionalityToButton(newColorButton, i);
            ChangeButtonColor(newColorButton, objectTypeSO.normalMaterials[i].color);
        }
    }

    #region ButtonSetup

    private void AssignColorUsingSpecificPixelOfTexture()
    {
        foreach (var material in objectTypeSO.normalMaterials)
        {
            var materialMainTexture = material.mainTexture;
            const int safePixelForPicking = 15;
            var pixelColor = ((Texture2D)materialMainTexture).GetPixel(safePixelForPicking, safePixelForPicking);
            material.color = pixelColor;
        }
    }

    private void AssignFunctionalityToButton(Button button, int buttonIndex)
    {
        button.onClick.AddListener(() =>
        {
            Debug.Log($"spawned button count: {spawnedButtons.Count}");

            objectTypeSO.prefab.gameObject.GetComponent<Renderer>().sharedMaterial = objectTypeSO.normalMaterials[buttonIndex];
            OnActiveObjectUpdated?.Invoke(this, new OnActiveObjectTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
            foreach (var spawnedButton in spawnedButtons)
            {
                spawnedButton.GetComponent<Outline>().enabled = false;
            }

            button.GetComponent<Outline>().enabled = true;
        });
    }

    private void ChangeButtonColor(Button button, Color color)
    {
        ColorBlock newColorBlock = button.colors;
        newColorBlock.normalColor = color;
        newColorBlock.disabledColor = color;
        newColorBlock.highlightedColor = color;
        newColorBlock.pressedColor = color;
        newColorBlock.selectedColor = color;
        button.colors = newColorBlock;
    }

    private void SetButtonPosition(Button button, int buttonIndex)
    {
        button.transform.SetParent(targetTransformParent);
        button.GetComponent<RectTransform>().anchoredPosition = anchoredPosition + new Vector2(0, -(buttonIndex + 1) * GapBetweenButtons);
    }

    #endregion
}

public class OnActiveObjectTypeChangedEventArgs
{
    public ObjectTypeSO activeObjectTypeSO;
}