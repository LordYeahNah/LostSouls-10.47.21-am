using System;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Task
{
    public Sequence(BehaviorTree tree) : base(tree)
    {
    }

    public Sequence(BehaviorTree tree, List<Task> children) : base(tree, children)
    {
    }

    public override ETaskStatus RunTask()
    {
        foreach (var child in _Children)
        {
            switch (child.RunTask())
            {
                case ETaskStatus.FAILED:
                    return ETaskStatus.FAILED;
                case ETaskStatus.SUCCESS:
                    continue;
                case ETaskStatus.RUNNING:
                    return ETaskStatus.SUCCESS;
            }
        }

        return ETaskStatus.FAILED;
    }
}