using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelEntrance : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _InteractableMessage;
    [SerializeField] private string _LevelName;
    
    public void Interact()
    {
        SceneManager.LoadScene(_LevelName, LoadSceneMode.Single);
        GameController.Instance.SaveGame.SaveGame(_LevelName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
                player._Interactable = this;
            
            if(_InteractableMessage)
                _InteractableMessage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
                player._Interactable = null;
            
            if(_InteractableMessage)
                _InteractableMessage.SetActive(false);
        }
    }
}
