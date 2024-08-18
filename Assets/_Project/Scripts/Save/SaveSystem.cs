using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static void Save(string fileName, string saveString, bool overwrite, int levelIndex)
    {
        Initiate();
        string saveFileName = fileName;
        // if (!overwrite)
        // {
        //     // int saveNumber = 1;

        //     while (File.Exists(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION))
        //     {

        //         // saveNumber++;
        //         // saveFileName = fileName + "_" + levelIndex;
        //     }
        // }

        File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION, saveString);

        Debug.Log($"saved with level index of {levelIndex}");
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
        return saveString;
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

    public static void SaveObject(object saveObject, int levelIndex)
    {
        SaveObject($"save_{LevelEditorManager.Instance.LevelIndex}", saveObject, true, levelIndex);
    }

    public static void SaveObject(string fileName, object saveObject, bool overwrite, int levelIndex)
    {
        Initiate();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite, levelIndex);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>()
    {
        Initiate();
        // string saveString = LoadMostRecentFile();
        string saveString = LoadSpecificFile($"save_{LevelEditorManager.Instance.LevelIndex}.txt");
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