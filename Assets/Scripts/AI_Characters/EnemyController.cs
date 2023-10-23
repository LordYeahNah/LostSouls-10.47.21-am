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
    }

    private void WithinTargetRange()
    {
        float currentDistance = Vector2.Distance(this.transform.position, _PlayerRef.transform.position);
        if (currentDistance < _TargetDistance)
        {
            Vector2 right = this.transform.TransformDirection(Vector2.right);
            Vector2 toOther = this.transform.position - _PlayerRef.transform.position;
            if (_Render.flipX)
            {
                if (Vector2.Dot(right, toOther) > 0)
                {
                    _Blackboard.SetValue("HasTarget", true);
                    _Blackboard.SetValue("Target", _PlayerRef.gameObject);
                }
            }
            else
            {
                if (Vector2.Dot(right, toOther) < 0)
                {
                    _Blackboard.SetValue("HasTarget", true);
                    _Blackboard.SetValue("Target", _PlayerRef.gameObject);
                }
            }
        }
    }

    protected override void CreateBlackboard()
    {
        base.CreateBlackboard();
        _Blackboard.SetValue("HasTarget", false);
    }

    public void TakeDamage(float dp, Vector3 attackPosition, float attackForce)
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
            
            ApplyAttackForce(attackPosition, attackForce);
        }
    }

    private void ApplyAttackForce(Vector3 attackPosition, float attackForce)
    {
        Vector2 right = this.transform.TransformDirection(Vector2.right);
        Vector2 otherPosition = this.transform.position - attackPosition;
        Vector2 forceToApply = Vector2.zero;
        
        if (Vector2.Dot(Vector2.right, otherPosition) > 0)
        {
            forceToApply = this.transform.TransformDirection(Vector2.right) * attackForce;
        }
        else
        {
            forceToApply = this.transform.TransformDirection(-Vector2.zero) * attackForce;
        }
        
        _RBody.AddForce(forceToApply * Time.deltaTime);
    }

    protected IEnumerator DeathDestroy()
    {
        yield return new WaitForSeconds(_DestroyTime);
        Destroy(this.gameObject);
    }
}