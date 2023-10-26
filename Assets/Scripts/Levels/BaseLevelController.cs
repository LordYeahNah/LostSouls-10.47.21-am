using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevelController : MonoBehaviour
{
    public static BaseLevelController Instance;

    private GameObject _SpawnedPlayer;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform _PlayerSpawnPoint;                   // Reference to where the player should spawn at

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else 
            DestroyImmediate(this.gameObject);
        
        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
        if (_PlayerSpawnPoint)
        {
            _SpawnedPlayer = GameObject.Instantiate(GameController.Instance.GetPlayerCharacter());
            if (_SpawnedPlayer != null)
            {
                _SpawnedPlayer.transform.position = _PlayerSpawnPoint.position;
                Camera.main.GetComponent<CameraController>()?.SetTarget(_SpawnedPlayer.transform);
            }
                
        }
    }
}