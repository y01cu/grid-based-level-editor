using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Basic button class ready with on click sound effect
/// </summary>
public class BasicButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => AudioManager.Instance.PlayBasicButtonClickSound());
    }
}
