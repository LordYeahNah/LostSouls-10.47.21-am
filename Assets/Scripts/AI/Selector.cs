using System;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Task
{
    public Selector(BehaviorTree tree) : base(tree)
    {
    }

    public Selector(BehaviorTree tree, List<Task> children) : base(tree, children)
    {
    }

    public override ETaskStatus RunTask()
    {
        foreach (var child in _Children)
        {
            switch (child.RunTask())
            {
                case ETaskStatus.FAILED:
                    continue;
                case ETaskStatus.RUNNING:
                    return ETaskStatus.RUNNING;
                case ETaskStatus.SUCCESS:
                    return ETaskStatus.SUCCESS;
            }
        }

        return ETaskStatus.FAILED;
    }
}