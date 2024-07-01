using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : IRMBListener
{
    public static LevelManager Instance;

    [SerializeField] private int moveCount;
    private int sceneBuildIndex;
    private int activeNonGrayCellCount;
    private int levelNumber;
    private float timer;
    private float activationInitialDelay = 3;
    private bool isGameEnded;

    #region Singleton

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

    #endregion

    private void Start()
    {
        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        levelNumber = sceneBuildIndex + 1;

        if (!isSubscribed)
        {
            ListenRMBEvent();
        }
    }

    private void Update()
    {
        if (isGameEnded)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= activationInitialDelay)
        {
            if (activeNonGrayCellCount == 0)
            {
                if (levelNumber == SceneManager.sceneCountInBuildSettings)
                {
                    UIManager.Instance.gameCompletedImage.gameObject.SetActive(true);
                    UIManager.Instance.gameCompletedImage.transform.DOScale(transform.localScale, 1f).From(Vector3.zero);
                    isGameEnded = true;
                }
                else
                {
                    SceneManager.LoadSceneAsync(levelNumber);
                }
            }
        }
    }

    public void DecreaseMoveCount()
    {
        moveCount--;

        if (moveCount < 0)
        {
            HandleLevelFail();
        }
    }

    private void HandleLevelFail()
    {
        UIManager.Instance.levelFailedImage.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(TweenFailedImage).AppendInterval(2f).onComplete += RestartLevel;
    }

    private void TweenFailedImage()
    {
        UIManager.Instance.levelFailedImage.transform.DOScale(transform.localScale, 1f).From(Vector3.zero);
    }

    private void RestartLevel()
    {
        SceneManager.LoadSceneAsync(levelNumber - 1);
    }

    public int GetRemaningMoveCount()
    {
        return moveCount;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    protected override void ListenRMBEvent()
    {
        MouseManager.OnMoveUsed += DecreaseMoveCount;

        isSubscribed = true;
    }

    protected override void StopListeningRMBEvent()
    {
        MouseManager.OnMoveUsed -= DecreaseMoveCount;

        isSubscribed = false;
    }

    public void IncreaseActiveNonGrayCellCount()
    {
        activeNonGrayCellCount++;
    }

    public void DecreaseActiveNonGrayCellCount()
    {
        activeNonGrayCellCount--;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    protected override void OnDestroy()
    {
        StopListeningRMBEvent();
    }
}