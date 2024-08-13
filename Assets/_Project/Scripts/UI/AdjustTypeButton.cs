using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdjustTypeButton : BasicButton
{
    public static event EventHandler<OnActiveObjectTypeChangedEventArgs> OnActiveObjectUpdated;
    public static event EventHandler OnHideAllMaterialButtons;

    [SerializeField] private ObjectTypeSO objectTypeSO;
    [SerializeField] private Button colorButtonTemplate;
    [SerializeField] private Transform targetTransformParent;
    public bool IsSelected { get; set; }
    private Vector2 anchoredPosition;
    private List<Button> spawnedMaterialButtons = new();

    private const float GapBetweenButtons = 100f;

    private void Start()
    {
        AssignName();
        AssignMaterialColorUsingSpecificPixelOfTexture();
        GetComponent<Button>().onClick.AddListener(UpdateAsSelected);

        // Every type's initial material index must be set to 0
        objectTypeSO.materialIndex = 0;

        OnActiveObjectUpdated += AdjustTypeButton_OnActiveObjectUpdated;

        OnHideAllMaterialButtons += (sender, args) =>
        {
            HideObjectMaterialButtons();
        };
    }

    private void AssignName()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = objectTypeSO.name;
    }

    private void UpdateOtherAdjustTypeButtons()
    {
        foreach (Transform buttonTransform in transform.parent)
        {
            buttonTransform.Find("Outline").gameObject.SetActive(false);
            // buttonTransform.GetComponent<AdjustTypeButton>().IsSelected = false;
        }
        transform.Find("Outline").gameObject.SetActive(true);
        // IsSelected = true;
    }

    private void AdjustTypeButton_OnActiveObjectUpdated(object sender, OnActiveObjectTypeChangedEventArgs args)
    {
        // if (!IsSelected)
        // {
        //     HideObjectMaterialButtons();
        // }
        //---
        // if (args.activeObjectTypeSO == objectTypeSO)
        // {
        //     transform.Find("Outline").gameObject.SetActive(true);
        // }
        //---
        // else
        // {
        //     transform.Find("Outline").gameObject.SetActive(false);
        // }
    }

    private void SetupObjectMaterialColorButtons()
    {
        if (spawnedMaterialButtons.Count > 0)
        {
            foreach (var spawnedMaterialButton in spawnedMaterialButtons)
            {
                spawnedMaterialButton.gameObject.SetActive(true);
            }
        }
        else
        {
            var colorMaterialCountOfCurrentObject = objectTypeSO.normalMaterials.Count;
            anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            for (int i = 0; i < colorMaterialCountOfCurrentObject; i++)
            {
                var newColorButton = Instantiate(colorButtonTemplate);
                spawnedMaterialButtons.Add(newColorButton);
                newColorButton.GetComponent<AdjustSpriteButton>().materialIndex = i;
                newColorButton.gameObject.SetActive(true);
                SetButtonPosition(newColorButton, i);
                AssignFunctionalityToMaterialButton(newColorButton, i);
                ChangeButtonColor(newColorButton, objectTypeSO.normalMaterials[i].color);

                if (i == 0)
                {
                    newColorButton.GetComponent<Outline>().enabled = true;
                    objectTypeSO.prefab.gameObject.GetComponent<Renderer>().sharedMaterial = objectTypeSO.normalMaterials[i];
                }
            }
        }
    }

    public void UpdateAsSelected()
    {
        UpdateOtherAdjustTypeButtons();
        OnHideAllMaterialButtons?.Invoke(this, EventArgs.Empty);
        OnActiveObjectUpdated?.Invoke(this, new OnActiveObjectTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
        if (IsSelected)
        {
            IsSelected = false;
            HideObjectMaterialButtons();
            return;
        }
        IsSelected = true;
        SetupObjectMaterialColorButtons();
        // OnActiveObjectUpdated?.Invoke(this, new OnActiveObjectTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
    }

    private void HideObjectMaterialButtons()
    {
        foreach (var spawnedMaterialButton in spawnedMaterialButtons)
        {
            spawnedMaterialButton.gameObject.SetActive(false);
        }
    }

    #region ButtonSetup

    private void AssignMaterialColorUsingSpecificPixelOfTexture()
    {
        foreach (var material in objectTypeSO.normalMaterials)
        {
            var materialMainTexture = material.mainTexture;
            const int safePixelForPicking = 15;
            var pixelColor = ((Texture2D)materialMainTexture).GetPixel(safePixelForPicking, safePixelForPicking);
            material.color = pixelColor;
        }
    }

    private void AssignFunctionalityToMaterialButton(Button materialButton, int buttonIndex)
    {
        materialButton.onClick.AddListener(() =>
        {
            objectTypeSO.prefab.gameObject.GetComponent<Renderer>().sharedMaterial = objectTypeSO.normalMaterials[buttonIndex];
            objectTypeSO.materialIndex = buttonIndex; // Since the first material is the default material 

            Debug.Log($"obj type so has material index: {objectTypeSO.materialIndex}");

            OnActiveObjectUpdated?.Invoke(this, new OnActiveObjectTypeChangedEventArgs { activeObjectTypeSO = objectTypeSO });
            foreach (var spawnedButton in spawnedMaterialButtons)
            {
                spawnedButton.GetComponent<Outline>().enabled = false;
            }

            materialButton.GetComponent<Outline>().enabled = true;
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
        button.GetComponent<RectTransform>().anchoredPosition = anchoredPosition + new Vector2((buttonIndex + 1) * GapBetweenButtons, 0);
    }

    #endregion
}

public class OnActiveObjectTypeChangedEventArgs
{
    public ObjectTypeSO activeObjectTypeSO;
}