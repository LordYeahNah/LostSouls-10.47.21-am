using System.Collections.Generic;
using System.IO;
using Pathfinding.Util;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPlayerCharacter
{
    SWEEPER = 0,
    ASSASSIN = 1,
}

[System.Serializable]
public class SaveGameController
{
    public string SaveGameID = "";
    private GameController _Controller;
    
    public SaveGameController(GameController controller)
    {
        _Controller = controller;
    }

    public void SaveGame()
    {
        string levelName = SceneManager.GetActiveScene().name;
        SaveGame(levelName);
    }

    public void SaveGame(string levelName)
    {
        if (SaveGameID == "")
            SaveGameID = Guid.NewGuid().ToString();
        
        PlayerController player = _Controller.PlayerRef;
        PlayerSaveData data = null;
        if (player)
        {
            data = player.SavePlayerData();
            data.PlayerLevel = levelName;
        }

        if (data != null)
        {
            _Controller.SaveData = data;
            string dataJSON = data.ToJson();
            string dataPath = Application.persistentDataPath;
            dataPath += $"/{SaveGameID}.yns";

            using (StreamWriter writer = new StreamWriter(dataPath))
            {
                writer.WriteLine(dataJSON);
            }
        }
    }
}

public class PlayerSaveData
{
    public EPlayerCharacter Character;
    public float CurrentHealth;
    public Vector2 SavePosition;
    public string PlayerLevel;

    public PlayerSaveData(EPlayerCharacter character, float currentHealth, Vector2 position)
    {
        Character = character;
        CurrentHealth = currentHealth;
        SavePosition = position;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}