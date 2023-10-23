using System;
using UnityEngine;

public class HasMoveToLocation : Task
{
    public HasMoveToLocation(BehaviorTree tree) : base(tree)
    {
    }

    public override ETaskStatus RunTask()
    {
        if (!_Tree)
            return ETaskStatus.FAILED;

        Blackboard board = _Tree.GetBlackboard();
        if (board != null)
        {
            if (board.GetValue<bool>("HasMoveToLocation"))
                return ETaskStatus.SUCCESS;
        }

        return ETaskStatus.FAILED;
    }
}