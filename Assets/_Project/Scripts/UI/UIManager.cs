using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : IRMBListener
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI movesTMP;
    [SerializeField] private TextMeshProUGUI levelTMP;

    public Image levelFailedImage;
    public Image gameCompletedImage;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private StringBuilder stringBuilder;

    private WaitForSeconds initialDelayForSetup = new(0.5f);

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return initialDelayForSetup;

        if (!isSubscribed)
        {
            ListenRMBEvent();
        }

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

    protected override void ListenRMBEvent()
    {
        MouseManager.OnMoveUsed += DecreaseMovesText;

        isSubscribed = true;
    }

    protected override void StopListeningRMBEvent()
    {
        MouseManager.OnMoveUsed -= DecreaseMovesText;

        isSubscribed = false;
    }

    protected override void OnDestroy()
    {
        StopListeningRMBEvent();
    }
}