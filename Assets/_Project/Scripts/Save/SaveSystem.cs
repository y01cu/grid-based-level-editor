using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    private const string SAVE_EXTENSION = "txt";

    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private static bool isInitiated = false;


    public static void Initiate()
    {
        if (!isInitiated)
        {
            isInitiated = true;

            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }

    public static void SaveTemplate(string fileName, string saveString)
    {
        Initiate();
        File.WriteAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION, saveString);
    }

    public static void Save(string fileName, string saveString, bool overwrite, int levelIndex)
    {
        Initiate();
        string saveFileName = fileName;
        File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION, saveString);
    }

    public static string Load(string fileName)
    {
        Initiate();
        if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
            return saveString;
        }
        else
        {
            return null;
        }
    }
    public static string LoadSpecificFile(string fileName)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        string saveString = null;
        foreach (var fileInfo in saveFiles)
        {
            if (fileInfo.Name == fileName)
            {
                saveString = File.ReadAllText(fileInfo.FullName);
                return saveString;
            }
        }
        Debug.Log($"Failed to find file with name {fileName}");
        // then create a new one
        string newFilePath = Path.Combine(SAVE_FOLDER, fileName);
        using (FileStream fs = File.Create(newFilePath))
        {

        }
        var baseSaveString = File.ReadAllText(SAVE_FOLDER + "save_-1.txt");
        File.WriteAllText(SAVE_FOLDER + fileName, baseSaveString);
        return baseSaveString;
    }

    public static string LoadMostRecentFile()
    {
        Initiate();
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        FileInfo mostRecentFile = null;
        foreach (var fileInfo in saveFiles)
        {
            if (mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else
            {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = fileInfo;
                }
            }
        }

        if (mostRecentFile != null)
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static int GetHighestLevel()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        int highestLevel = 0;
        foreach (var fileInfo in saveFiles)
        {
            string[] splitFileName = fileInfo.Name.Split('_');
            if (splitFileName.Length > 1)
            {
                string secondPart = splitFileName[1];
                int levelIndex = int.Parse(secondPart.Split('.')[0]);
                if (levelIndex > highestLevel)
                {
                    highestLevel = levelIndex;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse level index from file name: {fileInfo.Name}");
                }
            }
            else
            {
                Debug.LogWarning($"Unexpected file name format: {fileInfo.Name}");
            }
        }
        Debug.Log($"highest saved level is {highestLevel}");
        return highestLevel;
    }

    public static void SaveObject(object saveObject, int levelIndex)
    {
        SaveObject(saveObject, true, levelIndex);
    }

    public static void SaveObject(object saveObject, bool overwrite, int levelIndex)
    {
        Initiate();
        string json = JsonUtility.ToJson(saveObject);
        Save($"save_{levelIndex}", json, overwrite, levelIndex);
    }

    public static TSaveObject LoadSaveObject<TSaveObject>()
    {
        int levelIndex;

        if (LevelEditorManager.Instance == null)
        {
            string activeSceneName = SceneManager.GetActiveScene().name;
            int.TryParse(activeSceneName[activeSceneName.Length - 1].ToString(), out int currentLevelIndex);
            levelIndex = currentLevelIndex;
        }
        else
        {
            levelIndex = LevelEditorManager.Instance.LevelIndex;
        }

        string saveString = LoadSpecificFile($"save_{levelIndex}.txt");
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }

        return default;
    }

    public static TSaveObject LoadObect<TSaveObject>(string fileName)
    {
        Initiate();
        string saveString = Load(fileName);
        if (saveString != null)
        {
            TSaveObject saveObject = JsonUtility.FromJson<TSaveObject>(saveString);
            return saveObject;
        }

        return default;
    }
}