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
                board.SetValue("MoveToLocation", new Vector2(target.transform.position.x, target.transform.position.y));
                return ETaskStatus.SUCCESS;
            }
        }

        return ETaskStatus.FAILED;
    }
}