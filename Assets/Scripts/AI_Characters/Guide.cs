using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Guide : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogController _Dialog;
    [SerializeField] private GameObject _InteractObject;
    [SerializeField] private GameObject _DialogObject;

    private PlayerController _Player;
    
    public void Interact()
    {
        if (!_Dialog.IsDialogActive)
        {
            if (_Dialog)
            {
                _Dialog.IsDialogActive = true;
                _InteractObject.SetActive(false);
                _DialogObject.SetActive(true);
            }
        }
        else
        {
            _Dialog.NextAction();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {  
        // Indicate interaction
        if(_InteractObject)
            _InteractObject.SetActive(true);
        
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
            {
                _Player = player;
                player._Interactable = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(_InteractObject)
            _InteractObject.SetActive(false);
        
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
            {
                _Player.IsInInteraction = false;
                _Player = null;
                player._Interactable = null;
            }
        }
    }

    public void FinishInteraction()
    {
        if (_Player)
        {
            _Player.IsInInteraction = false;
        }
    }
}
