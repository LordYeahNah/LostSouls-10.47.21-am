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
                new HasTarget(this),
                new IsInAttackDistance(this),
            }),
            new Sequence(this, new List<Task>
            {
                new HasTarget(this),
                new GetTargetPosition(this),
                new MoveToLocation(this)
            }),
            new Sequence(this, new List<Task>
            {
                new HasMoveToLocation(this),
                new MoveToLocation(this)
            })
        });
    }
}
