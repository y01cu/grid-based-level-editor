using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Subscriber
{
    public static LevelManager Instance;

    [SerializeField] private Image levelFailedImage;
    [SerializeField] private Image gameCompletedImage;

    [SerializeField] private int moveCount;
    private int sceneBuildIndex;
    private int activeNonGrayCellCount;
    private int levelNumber;

    private float timer;
    private float activationInitialDelay = 3;

    private bool isGameEnd;

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
            SubscribeToEvents();
        }
    }

    private void Update()
    {
        if (isGameEnd)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= activationInitialDelay)
        {
            if (activeNonGrayCellCount == 0)
            {
                Debug.Log("level no: " + levelNumber + " | scene count: " + SceneManager.sceneCountInBuildSettings);

                if (levelNumber == SceneManager.sceneCountInBuildSettings)
                {
                    gameCompletedImage.gameObject.SetActive(true);
                    gameCompletedImage.transform.DOScale(transform.localScale, 1f).From(Vector3.zero);
                    isGameEnd = true;
                }
                else
                {
                    SceneManager.LoadSceneAsync(levelNumber);
                }
            }
        }
    }

    private void ShowCompletedImage()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(TweenCompletedImage).AppendInterval(2f);
    }

    public void DecreaseMoveCount()
    {
        moveCount--;

        if (moveCount < 0)
        {
            levelFailedImage.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(TweenFailedImage).AppendInterval(2f).onComplete += RestartLevel;
        }
    }

    private void TweenFailedImage()
    {
        levelFailedImage.transform.DOScale(transform.localScale, 1f).From(Vector3.zero);
    }

    private void TweenCompletedImage()
    {
        gameCompletedImage.transform.DOScale(transform.localScale, 1f).From(Vector3.zero);
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

    protected override void SubscribeToEvents()
    {
        MouseManager.OnMoveUsed += DecreaseMoveCount;

        isSubscribed = true;
    }

    protected override void UnsubscribeFromEvents()
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
        Application.Quit();
    }

    protected override void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}