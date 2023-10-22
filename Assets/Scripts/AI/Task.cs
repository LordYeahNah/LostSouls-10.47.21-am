using System;
using System.Collections.Generic;
using UnityEngine;

public enum ETaskStatus
{
    FAILED = 0,
    SUCCESS = 1,
    RUNNING = 2,
}

public abstract class Task
{
    protected BehaviorTree _Tree;
    public Task Parent;
    protected List<Task> _Children = new List<Task>();

    public Task(BehaviorTree tree)
    {
        _Tree = tree;
    }

    public Task(BehaviorTree tree, List<Task> children)
    {
        _Tree = tree;
        foreach(var child in children)
            AttachChild(child);
    }

    public void AttachChild(Task child)
    {
        _Children.Add(child);
        child.Parent = this;
    }
}