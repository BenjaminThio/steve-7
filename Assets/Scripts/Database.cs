using UnityEngine;
using System.IO;

public static class Database
{
    private static string Path => Application.persistentDataPath + "/save.json";
    public static GameData data;

    public static void Load()
    {
        if (!File.Exists(Path))
        {
            GameData newGameData = new();

            data = newGameData;
            Save(newGameData);
        }

        string json = File.ReadAllText(Path);
        data = JsonUtility.FromJson<GameData>(json);
    }

    public static void Save(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(Path, json);
    }

    // Game Bootstrap
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        Debug.Log(Application.persistentDataPath);
        Save(new());
        Load();
    }
}