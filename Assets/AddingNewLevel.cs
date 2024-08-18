using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading.Tasks;

public class AddingNewLevel : MonoBehaviour
{
    [SerializeField] private Button addLevelButton;
    [SerializeField] private Button newLevelButton;
    [SerializeField] private Transform parentTransform;
    private int buttonCounter;

    private void Start()
    {
        addLevelButton.onClick.AddListener(AddNewLevel);
        AssignNumbersToLevelButtons();
    }

    public void AddNewLevel()
    {
        buttonCounter++;
        transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.05f).OnComplete(() =>
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), 0.05f).onComplete = () =>
            {
                transform.SetSiblingIndex(transform.parent.childCount - 2);
                Button newlyCreatedLevel = Instantiate(newLevelButton, parentTransform);
                newlyCreatedLevel.transform.SetSiblingIndex(parentTransform.childCount - 3);
                newlyCreatedLevel.gameObject.SetActive(true);
                newlyCreatedLevel.GetComponentInChildren<TextMeshProUGUI>().text = buttonCounter.ToString();
                newlyCreatedLevel.transform.DOScale(transform.localScale, 0.3f).From(Vector3.zero).SetEase(Ease.OutBounce);
            };
        });

    }

    private async void AssignNumbersToLevelButtons()
    {
        await Task.Delay(400);

        for (int i = 0; i < 6; i++)
        {
            await Task.Delay(300);
            AddNewLevel();
        }
    }
}
