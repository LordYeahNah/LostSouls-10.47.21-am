using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Ionic.Zip;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sweeper : PlayerController
{
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
        if (_SpinAttackEnabled)
            _SpinAttackEnabled = false;
    }
    
    public void OnPauseSlamAnimation()
    {
        Debug.Log("Pausing Attack");
        _Anim.speed = 0f;
        _IsInSlamAttack = true;
    }
}