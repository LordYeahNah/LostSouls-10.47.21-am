using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AssassinAnimHelper : MonoBehaviour
{
    public Assassin _Player;                        // Reference to the player character

    public void PerformAttack()
    {
        if(_Player)
            _Player.PerformAttack();
    }
}
