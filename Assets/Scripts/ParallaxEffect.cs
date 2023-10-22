using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float _StartXPosition;                  // reference to where the background starts
    private float _LengthOfSprite;                          // The size of the sprite on the x
    [SerializeField] private float _ParallaxAmount;
    [SerializeField] private Transform _TargetTransform;
    [SerializeField] private SpriteRenderer _Sprite;                    // reference to this sprite render

    private void Awake()
    {
    }
    
    private void Start()
    {
        _StartXPosition = this.transform.position.x;
        _LengthOfSprite = _Sprite.bounds.size.x;
    }

    private void Update()
    {
        Vector3 position = _TargetTransform.transform.position;
        float temp = position.x * (0.5f - _ParallaxAmount);
        float distance = position.x * _ParallaxAmount;
        Vector3 newPosition =
            new Vector3(_StartXPosition + distance, this.transform.position.y, this.transform.position.z);

        transform.position = newPosition;

        if (temp > _StartXPosition + (_LengthOfSprite / 2))
        {
            _StartXPosition += _LengthOfSprite;
        } else if (temp < _StartXPosition - (_LengthOfSprite / 2))
        {
            _StartXPosition -= _LengthOfSprite;
        }
    }
}