using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : AIController
{
    [Header("Target Settings")]
    [SerializeField] protected PlayerController _PlayerRef;                        // store reference to the player
    [SerializeField] protected float _TargetDistance;                     // How far the target should be before seeing

    [Header("Stats")] 
    [SerializeField] protected float _MaxHealth;
    [SerializeField] protected float _CurrentHealth;

    [Header("Components")] 
    [SerializeField] protected GameObject _EyeLight;                    // Reference to the eyes light of the enemy

    [Header("Destroy Settings")] 
    [SerializeField] protected float _DestroyTime = 0.5f;

    public bool IsAlive => _CurrentHealth > 0;

    protected override void Awake()
    {
        base.Awake();
        _CurrentHealth = _MaxHealth;
    }
    
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

    public void TakeDamage(float dp)
    {
        _CurrentHealth -= dp;
        if (_CurrentHealth <= 0)
        {
            if(_Anim)
                _Anim.SetTrigger("Death");

            _Tree.CanUpdate = false;
            _EyeLight.SetActive(false);
            StartCoroutine(DeathDestroy());
            _Path = null;
            _RBody.velocity = Vector2.zero;
        }
        else
        {
            if(_Anim)
                _Anim.SetTrigger("TakeHit");
        }
    }

    protected IEnumerator DeathDestroy()
    {
        yield return new WaitForSeconds(_DestroyTime);
        Destroy(this.gameObject);
    }
}