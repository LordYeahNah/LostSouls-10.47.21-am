using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : AIController
{
    [Header("Target Settings")]
    [SerializeField] protected PlayerController _PlayerRef;                        // store reference to the player
    [SerializeField] protected float _TargetDistance;                     // How far the target should be before seeing
    

    protected override void Start()
    {
        base.Start();
        
        InvokeRepeating(nameof(WithinTargetRange), 0f, 1.0f);
    }

    private void Update()
    {
        if (_PlayerRef)
        {
            Vector2 enemyWorldDirection = this.transform.TransformDirection(Vector2.right);
            Vector2 playerDirection = this.transform.position - _PlayerRef.transform.position;
            Debug.Log(Vector2.Dot(enemyWorldDirection, playerDirection));
        }
    }

    private void WithinTargetRange()
    {
        float currentDistance = Vector2.Distance(this.transform.position, _PlayerRef.transform.position);
        if (currentDistance < _TargetDistance)
        {
            if (_Blackboard != null)
            {
                _Blackboard.SetValue("HasTarget", true);
                _Blackboard.SetValue("Target", _PlayerRef.gameObject);
            }
        }
    }

    protected override void CreateBlackboard()
    {
        base.CreateBlackboard();
        _Blackboard.SetValue("HasTarget", false);
    }
}