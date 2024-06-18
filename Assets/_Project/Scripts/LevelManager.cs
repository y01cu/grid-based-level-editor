using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int sceneBuildIndex;

    private float timer;
    private float activationInitialDelay = 3;
    private int activeNonGrayCellCount;

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
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= activationInitialDelay)
        {
            if (activeNonGrayCellCount == 0 && sceneBuildIndex != SceneManager.sceneCountInBuildSettings - 1)
            {
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

    public void LogActiveNonGrayCellCount()
    {
        Debug.Log("active non gray cell count: " + activeNonGrayCellCount);
    }
}