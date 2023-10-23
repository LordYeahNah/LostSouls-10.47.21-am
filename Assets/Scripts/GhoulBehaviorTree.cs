using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulBehaviorTree : BehaviorTree
{
    protected override Task CreateTree()
    {
        return new Selector(this, new List<Task>
        {
            new Sequence(this, new List<Task>
            {
                new HasMoveToLocation(this),
                new MoveToLocation(this)
            })
        });
    }
}
