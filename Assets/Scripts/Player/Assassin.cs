using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EAssassinAttacks
{
    STANDARD = 0,
    HEAVY = 1,
    DUAL = 2
}

public class Assassin : PlayerController
{
    [Header("Standard Attack")] 
    [SerializeField] protected float _StandardAttackCooldown;
    [SerializeField] protected float _AttackDPModifier = 1.0f;

    [Header("Heavy Attack")] 
    [SerializeField] protected float _HeavyAttackCooldown;
    [SerializeField] protected float _HeavyAttackDPModifier = 1.0f;

    [Header("Dual Attack")] 
    [SerializeField] protected float _DualAttackCooldown;
    [SerializeField] protected float _DualAttackDPModifier;
    private bool _IsDualAttack;

    private EAssassinAttacks _CurrentAttack;                            // Attack currently being performed

    public override void PerformAttack()
    {
        base.PerformAttack();
        Vector2 direction = Vector2.zero;
        if (_Flipped)
        {
            direction = transform.TransformDirection(Vector2.right);
        }
        else
        {
            direction = transform.TransformDirection(-Vector2.right);
        }

        RaycastHit2D hit = Physics2D.Raycast(_AttackCastTransform.position, direction, _AttackDistance, _DamageMask);
        if (hit)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyController>()?.TakeDamage(GenerateDamagePoints(),
                    _Rbody.position, GetAttackForce());
            }
        }
    }

    protected override void GenerateAttack()
    {
        if (IsGrounded())
        {
            if (_IsDualAttack)
            {
                _CurrentAttack = EAssassinAttacks.DUAL;
                StartCoroutine(AttackCooldown(_DualAttackCooldown));
            }
            else
            {
                if (_HeavyAttack)
                {
                    _CurrentAttack = EAssassinAttacks.HEAVY;
                    StartCoroutine(AttackCooldown(_HeavyAttackCooldown));
                }
                else
                {
                    _CurrentAttack = EAssassinAttacks.STANDARD;
                    StartCoroutine(AttackCooldown(_StandardAttackCooldown));
                }
            }
        }
    }
    
    

    protected override float GenerateDamagePoints()
    {
        bool isCritHit = Random.Range(0f, 1f) < _CriticalHitChance;
        float damagePoints = Random.Range(_DamagePoints - 4, _DamagePoints);
        if (isCritHit)
            damagePoints *= _CriticalHitModifier;

        if (_CurrentAttack == EAssassinAttacks.DUAL)
            return damagePoints * _DualAttackDPModifier;

        if (_CurrentAttack == EAssassinAttacks.HEAVY)
            return damagePoints * _HeavyAttackDPModifier;

        if (_CurrentAttack == EAssassinAttacks.STANDARD)
            return damagePoints * _StandardAttackCooldown;

        return damagePoints;
    }

    protected override void AttackReset()
    {
        base.AttackReset();
        if (_IsDualAttack)
            _IsDualAttack = false;
    }

    protected override float GetAttackForce()
    {
        return 0f;
    }

    public void EnableDualAttack(InputAction.CallbackContext ctx)
    {
        _IsDualAttack = ctx.performed;
        if(_Anim)
            _Anim.SetBool("DualAttack", _IsDualAttack);
    }

    public override void Attack(InputAction.CallbackContext ctx)
    {
        if (!IsGrounded())
            return;
        
        base.Attack(ctx);
    }
}
