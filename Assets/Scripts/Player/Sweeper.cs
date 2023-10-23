using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Ionic.Zip;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sweeper : PlayerController
{
    
    [Header("Sweep Attack")] 
    [SerializeField] protected float _SweepAttackCooldown;
    [SerializeField] protected float _SweepAttackForce;                           // How much force to be applied to the enemy when struct        
    [SerializeField] protected float _SweepAttackDPModifier = 1.0f;
    
    [Header("Slam Attack")]
    [SerializeField] protected float _SlamAttackCooldown;
    [SerializeField] protected float _GroundDistanceToAttack;                     // How far is required to be before finishing the attack
    [SerializeField] protected LayerMask _SlamLayer;
    [SerializeField] protected float _SlammAttackDPModifier = 1.0f;
    [SerializeField] protected float _SlamAttackForce;
    protected bool _IsInSlamAttack = false;
    

    [Header("Spin Attack")] 
    [SerializeField] protected float _SpinAttackCooldown = 0.35f;
    [SerializeField] protected float _SpinAttackForce;
    [SerializeField] protected float _SpinAttackDPModifier = 1.0f;
    protected bool _SpinAttackEnabled = false;                        // If we are wanting to perform a spin attack

    protected override void Update()
    {
        base.Update();
        if (_IsInSlamAttack)
        {
            if (Physics2D.OverlapCircle(_GroundedCheckTransform.position, 
                    _GroundDistanceToAttack, _SlamLayer))
            {
                if (_Anim)
                    _Anim.speed = 1;

                _IsInSlamAttack = false;
            }
        }
    }

    protected override void GenerateAttack()
    {
        if (IsGrounded())
        {
            if (!_SpinAttackEnabled)
            {
                _AttackType = ESweeperAttackType.SWEEP;
                StartCoroutine(AttackCooldown(_SweepAttackCooldown));
            }
            else
            {
                StartCoroutine(AttackCooldown(_SpinAttackCooldown));
                _AttackType = ESweeperAttackType.SPIN;
            }
        }
        else
        {
            StartCoroutine(AttackCooldown(_SlamAttackCooldown));
            _AttackType = ESweeperAttackType.SLAM;
        }
    }

    protected override float GenerateDamagePoints()
    {
        bool isCriticalHit = Random.Range(0f, 1f) < _CriticalHitChance;
        float damagePoints = Random.Range((_DamagePoints - 4), _DamagePoints);
        if (isCriticalHit)
            damagePoints *= _CriticalHitModifier;

        if (_AttackType == ESweeperAttackType.SPIN)
        {
            damagePoints *= _SpinAttackDPModifier;
        } else if (_AttackType == ESweeperAttackType.SLAM)
        {
            damagePoints *= _SlammAttackDPModifier;
        }
        else
        {
            damagePoints *= _SweepAttackDPModifier;
        }

        return damagePoints;
    }

    protected override float GetAttackForce()
    {
        switch (_AttackType)
        {
            case ESweeperAttackType.SLAM:
                return _SlamAttackForce;
            case ESweeperAttackType.SPIN:
                return _SpinAttackForce;
            case ESweeperAttackType.SWEEP:
                return _SweepAttackForce;
        }

        return 0f;
    }

    protected override void AttackReset()
    {
        base.AttackReset();
        if (_HeavyAttack)
            _HeavyAttack = false;
    }
    
    public void OnPauseSlamAnimation()
    {
        _Anim.speed = 0f;
        _IsInSlamAttack = true;
    }
}