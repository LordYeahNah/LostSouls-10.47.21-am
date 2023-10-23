using System;
using System.Collections.Generic;
using UnityEngine;

public class HasTarget : Task
{
    public HasTarget(BehaviorTree tree) : base(tree)
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
                return ETaskStatus.SUCCESS;
            }
        }

        return ETaskStatus.FAILED;
    }
}