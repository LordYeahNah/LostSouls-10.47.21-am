using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorTree : MonoBehaviour
{
    protected Task _RootTask;
    protected Blackboard _Blackboard;

    protected bool _Initialized = false;

    private void Awake()
    {
        _RootTask = CreateTree();
    }

    public void Initialize(Blackboard board)
    {
        _Blackboard = board;
        _Initialized = true;
    }

    protected virtual void Update()
    {
        if (!_Initialized || _RootTask == null)
            return;

        _RootTask.RunTask();
    }

    protected abstract Task CreateTree();

    public Blackboard GetBlackboard() => _Blackboard;
}