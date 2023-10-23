using System.Collections.Generic;
using System;
using UnityEngine;

public class GetTargetPosition : Task
{
    public GetTargetPosition(BehaviorTree tree) : base(tree)
    {
    }

    public override ETaskStatus RunTask()
    {
        if (!_Tree)
            return ETaskStatus.FAILED;

        Blackboard board = _Tree.GetBlackboard();
        if (board != null)
        {
            GameObject target = board.GetValue<GameObject>("Target");
            if (target)
            {
                board.SetValue("MoveToLocation", target.transform.position);
                return ETaskStatus.SUCCESS;
            }
        }

        return ETaskStatus.FAILED;
    }
}