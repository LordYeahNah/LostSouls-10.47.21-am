using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevelController : MonoBehaviour
{
    public static BaseLevelController Instance;
    
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
            GameObject player = GameController.Instance.GetPlayerCharacter();
            if (player != null)
                player.transform.position = _PlayerSpawnPoint.position;
        }
    }
}