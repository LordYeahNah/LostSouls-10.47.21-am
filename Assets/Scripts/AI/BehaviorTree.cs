using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorTree : MonoBehaviour
{
    protected Task _RootTask;

    private void Awake()
    {
        _RootTask = CreateTree();
    }

    protected abstract Task CreateTree();
}