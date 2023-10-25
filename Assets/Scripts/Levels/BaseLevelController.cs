using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevelController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform _PlayerSpawnPoint;                   // Reference to where the player should spawn at

    private void Awake()
    {
        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
        if (_PlayerSpawnPoint)
        {
            GameObject spawnedPlayer = Instantiate(GameController.Instance.PlayerObject);
            if (spawnedPlayer)
                spawnedPlayer.transform.position = _PlayerSpawnPoint.transform.position;
        }
    }
}