using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : AIController
{
    [Header("Target Settings")]
    private PlayerController _PlayerRef;                        // store reference to the player
    
    

    protected override void Start()
    {
        base.Start();
        
        InvokeRepeating(nameof(WithinTargetRange), 0f, 1.0f);
    }

    private bool WithinTargetRange()
    {
        float currentDistance = Vector2.Distance(this.transform.position, _PlayerRef.transform.position);
        
    }
}