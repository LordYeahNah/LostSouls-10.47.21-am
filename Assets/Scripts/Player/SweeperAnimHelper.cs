using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweeperAnimHelper : MonoBehaviour
{
    [SerializeField] private Sweeper _Player;

    public void OnPauseSlam()
    {
        if(_Player)
            _Player.OnPauseSlamAnimation();
    }

    public void PerformAttack()
    {
        if(_Player)
            _Player.PerformAttack();
    }
}
