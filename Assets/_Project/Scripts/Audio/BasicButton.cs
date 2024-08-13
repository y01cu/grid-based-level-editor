using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Basic button class ready with on click sound effect
/// </summary>
public class BasicButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayBasicButtonClickSound();
            transform.DOScale(1.1f, 0.05f).OnComplete(() =>
            {
                transform.DOScale(1f, 0.05f);
            });
        });
    }
}
