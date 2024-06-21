using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelManager : Subscriber
{
    public static LevelManager Instance;
    private int sceneBuildIndex;

    private float timer;
    private float activationInitialDelay = 3;
    private int activeNonGrayCellCount;

    private int levelNumber;

    [SerializeField] private int moveCount;

    [SerializeField] private Image levelFailedImage;

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public void DecreaseMoveCount()
    {
        Debug.Log("move count decreased");
        moveCount--;

        if (moveCount <= 0)
        {
            levelFailedImage.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(TweenFailedImage).AppendInterval(2f).onComplete += RestartLevel;

            // onComplete += sequence.AppendInterval(0.5f).onComplete += RestartLevel;
        }
    }

    private void TweenFailedImage()
    {
        levelFailedImage.transform.DOScale(transform.localScale, 1f).From(Vector3.zero);
    }

    private void RestartLevel()
    {
        SceneManager.LoadSceneAsync(levelNumber - 1);
    }

    public int GetRemaningMoveCount()
    {
        return moveCount;
    }

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

    private void Start()
    {
        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        levelNumber = sceneBuildIndex + 1;

        if (!isSubscribed)
        {
            SubscribeToEvents();
        }
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

    protected override void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= activationInitialDelay)
        {
            if (activeNonGrayCellCount == 0 && sceneBuildIndex != SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadSceneAsync(levelNumber);
            }
        }
    }

    public void IncreaseActiveNonGrayCellCount()
    {
        activeNonGrayCellCount++;
    }

    public void DecreaseActiveNonGrayCellCount()
    {
        activeNonGrayCellCount--;
    }
}