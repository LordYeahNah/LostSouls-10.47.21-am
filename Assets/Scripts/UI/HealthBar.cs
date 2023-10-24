using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerController _Player;
    [SerializeField] private Image _HealthSlider;

    private void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if(_Player)
            _Player._CharacterTakeDamage.AddListener(UpdateHealthBar);
        
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (_Player)
        {
            float currentHealth = _Player.CurrentHealth;
            float percentage = currentHealth / 100;
            if (_HealthSlider)
                _HealthSlider.fillAmount = percentage;
        }
    }
}
