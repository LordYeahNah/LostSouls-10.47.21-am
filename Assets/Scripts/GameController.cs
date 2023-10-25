using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Player Objects")] 
    [SerializeField] private GameObject _Sweeper;

    private PlayerSaveData _SaveData;                       // Reference to the characters current data
    private PlayerController _PlayerRef;                            // Reference to the player

    public PlayerController PlayerRef => _PlayerRef;

    private SaveGameController _SaveGame;
    public SaveGameController SaveGame => _SaveGame;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }

        _SaveGame = new SaveGameController(this);
        

        if (!_PlayerRef)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                _PlayerRef = player.GetComponent<PlayerController>();
        }
    }

    public GameObject GetPlayerCharacter()
    {
        GameObject toSpawn = null;
        if (_SaveData != null)
        {
            switch (_SaveData.Character)
            {
                case EPlayerCharacter.SWEEPER:
                    toSpawn = _Sweeper;
                    break;
            }
        }

        if (!toSpawn)
        {
            PlayerController player = toSpawn.GetComponent<PlayerController>();
            if (player)
            {
                player.SetupData(_SaveData);
                _PlayerRef = player;
            }
                
        }

        return toSpawn;
    }
}