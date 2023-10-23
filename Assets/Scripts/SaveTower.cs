using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveTower : MonoBehaviour
{
    [SerializeField] private GameObject _SavePanel;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_SavePanel)
                _SavePanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(_SavePanel)
                _SavePanel.SetActive(false);
        }
    }
}