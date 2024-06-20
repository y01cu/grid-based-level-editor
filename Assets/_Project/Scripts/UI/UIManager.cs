using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISubscriber
{
    [SerializeField] private TextMeshProUGUI movesTMP;
    [SerializeField] private TextMeshProUGUI levelTMP;

    private StringBuilder stringBuilder;

    private WaitForSeconds initialDelayForSetup = new(0.5f);

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return initialDelayForSetup;

        SubscribeToEvents();

        stringBuilder = new StringBuilder();

        // Setup initial TMPs

        movesTMP.text = UpdateMovesTMP(LevelManager.Instance.GetRemaningMoveCount());

        levelTMP.text = SetupLevelTMP(LevelManager.Instance.GetLevelNumber());
    }

    private async void DecreaseMovesText()
    {
        await Task.Delay(200);
        movesTMP.text = UpdateMovesTMP(LevelManager.Instance.GetRemaningMoveCount());
    }

    private string UpdateMovesTMP(int moveCount)
    {
        stringBuilder.Clear();

        stringBuilder.Append(moveCount);
        stringBuilder.Append(" MOVES");

        return stringBuilder.ToString();
    }

    private string SetupLevelTMP(int levelNumber)
    {
        stringBuilder.Clear();

        stringBuilder.Append("LEVEL ");
        stringBuilder.Append(levelNumber);

        return stringBuilder.ToString();
    }

    public void SubscribeToEvents()
    {
        MouseManager.OnMoveUsed += DecreaseMovesText;
        MouseManager.OnClearEvents += UnsubscribeFromEvents;
    }

    public void UnsubscribeFromEvents()
    {
        MouseManager.OnMoveUsed -= DecreaseMovesText;
        MouseManager.OnClearEvents -= UnsubscribeFromEvents;
    }
}