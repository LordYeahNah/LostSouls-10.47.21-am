using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class IsInAttackDistance : Task
{
    public IsInAttackDistance(BehaviorTree tree) : base(tree)
    {
    }

    public override ETaskStatus RunTask()
    {
        if (!_Tree)
            return ETaskStatus.FAILED;

        Blackboard board = _Tree.GetBlackboard();
        if (board != null)
        {
            if (board.GetValue<bool>("HasTarget"))
            {
                GameObject self = board.GetValue<GameObject>("Self");
                GameObject target = board.GetValue<GameObject>("Target");
                Vector2 targetPosition = target.transform.position;
                Vector2 agentPosition = self.transform.position;

                if (Vector2.Distance(targetPosition, agentPosition) < board.GetValue<float>("MinAttackDistance"))
                {
                    EnemyController enemy = self.GetComponent<EnemyController>();
                    if (enemy)
                        enemy.PerformAttack(target);

                    return ETaskStatus.SUCCESS;
                }
            }
        }

        return ETaskStatus.FAILED;
    }
}