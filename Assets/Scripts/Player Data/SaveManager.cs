using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string saveFileName = "playerSave.json";

    //Keep SaveData in memory once loaded in
    public static SaveData currentData = null;

    //Path to file
    private static string FullPath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, saveFileName);
        }
    }

    public static void LoadGame()
    {
        //if none exists, create new data
        if(!File.Exists(FullPath))
        {
            currentData = new SaveData();
            SaveGame(); //creatre default file
            Debug.Log("No save file found, created new one.");
            return;
        }

        //read JSON
        if (currentData == null)
        {
            //if could'nt parse, fallback
            currentData = new SaveData();
            Debug.Log("Failed to parse save, making new data");
        }
        else 
        {
            Debug.Log("Game loaded from: " + FullPath);
        }
    }

    public static void SaveGame()
    {
        if (currentData == null)
        {
            currentData = new SaveData();
        }
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(FullPath, json);
        Debug.Log("Game saved to: " + FullPath);
    }

}
