using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour, ISubscriber
{
    public static LevelManager Instance;
    private int sceneBuildIndex;

    private float timer;
    private float activationInitialDelay = 3;
    private int activeNonGrayCellCount;

    private int levelNumber;

    [SerializeField] private int moveCount;

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public void DecreaseMoveCount()
    {
        Debug.Log("move count decreased");
        moveCount--;
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

        SubscribeToEvent();
    }

    private void SubscribeToEvent()
    {
        MouseManager.OnMoveUsed += DecreaseMoveCount;
        MouseManager.OnClearEvents -= UnsubscribeFromEvents;
    }

    public void UnsubscribeFromEvent()
    {
        MouseManager.OnMoveUsed -= DecreaseMoveCount;
        MouseManager.OnClearEvents -= UnsubscribeFromEvents;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= activationInitialDelay)
        {
            if (activeNonGrayCellCount == 0 && sceneBuildIndex != SceneManager.sceneCountInBuildSettings - 1)
            {
                MouseManager.TriggerClearingEvents();
                SceneManager.LoadSceneAsync(sceneBuildIndex + 1);
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

    public void SubscribeToEvents()
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeFromEvents()
    {
        throw new NotImplementedException();
    }
}