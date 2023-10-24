using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    [SerializeField] private GameObject _NoteMessageObject;                     // Object that displays the note
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _NoteMessageObject.SetActive(true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _NoteMessageObject.SetActive((false));
        }
    }
}
