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

    [Header("Combat Settings")] 
    [SerializeField] protected float _AttackDistance = 0.5f;                        // How far the AI needs to be to attack the character
    [SerializeField] protected bool _CanAttack = true;                         // If the character can perform an attack
    [SerializeField] protected Transform _AttackCast;                           // where the attack cast starts
    [SerializeField] protected float _AttackCastDistance = 2f;
    [SerializeField] protected float _CooldownTime = 2f;                        // How long before the character can attack again
    [SerializeField] protected float _DamagePoints;
    [SerializeField] protected LayerMask _AttackLayer;
    
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
        _PlayerRef = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }
    
    protected override void Start()
    {
        base.Start();
        
        InvokeRepeating(nameof(WithinTargetRange), 0f, 1.0f);
    }

    private void Update()
    {
    }

    public virtual void PerformAttack(GameObject target)
    {
        if (!_CanAttack)
            return;
        
        Debug.Log("Attacking");

        if (_Anim)
            _Anim.SetTrigger("Attack");


        Vector2 direction = Vector2.zero;
        if (_Render.flipX)
        {
            direction = this.transform.TransformDirection(-Vector2.right);
        }
        else
        {
            direction = this.transform.TransformDirection(Vector2.right);
        }

        
        RaycastHit2D hit = Physics2D.Raycast(_AttackCast.position, direction, _AttackCastDistance, _AttackLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            PlayerController player = hit.collider.GetComponent<PlayerController>();    
            if (player != null)
            {
               player.TakeDamage(GenerateDamagePoints());
            }
        }

        _CanAttack = false;
        StartCoroutine(ResetAttack());
    }

    protected IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(_CooldownTime);
        _CanAttack = true;
    }

    protected virtual float GenerateDamagePoints()
    {
        return _DamagePoints;
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
        _Blackboard.SetValue("MinAttackDistance", _AttackDistance);
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

            // Set the target if it's not already set
            if (!_Blackboard.GetValue<bool>("HasTarget"))
            {
                _Blackboard.SetValue("HasTarget", true);
                _Blackboard.SetValue("Target", _PlayerRef.gameObject);
            }
            
           // ApplyAttackForce(attackPosition, attackForce);
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