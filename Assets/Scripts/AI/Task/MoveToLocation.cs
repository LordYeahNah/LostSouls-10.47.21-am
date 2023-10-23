using System;
using UnityEngine;

public class MoveToLocation : Task
{
    private const float POLL_TIME = 0.5f;
    private float _TimeSinceLastPoll = 0f;
    public MoveToLocation(BehaviorTree tree) : base(tree)
    {
    }

    public override ETaskStatus RunTask()
    {
        if (_Tree == null)
            return ETaskStatus.FAILED;

        Blackboard board = _Tree.GetBlackboard();
        if (board != null)
        {
            if (_TimeSinceLastPoll > POLL_TIME)
            {
                AIController controller = board.GetValue<GameObject>("Self").GetComponent<AIController>();
                if (controller)
                {
                    controller.UpdatePath(board.GetValue<Vector2>("MoveToLocation"));
                }

                _TimeSinceLastPoll = 0f;
            }

            _TimeSinceLastPoll += 1 * Time.deltaTime;
        }
    }
}