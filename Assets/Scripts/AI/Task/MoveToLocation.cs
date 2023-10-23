using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MoveToLocation : Task
{
    private const float STOPPING_DISTANCE = 0.5f;
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
            _TimeSinceLastPoll += 1 * Time.deltaTime;
            if (_TimeSinceLastPoll > POLL_TIME)
            {
                AIController controller = board.GetValue<GameObject>("Self").GetComponent<AIController>();
                Vector2 location = board.GetValue<Vector2>("MoveToLocation");
                float distance = Vector2.Distance(controller.transform.position, location);
                Debug.Log(distance);
                if (distance > STOPPING_DISTANCE)
                {
                    controller.UpdatePath(location);
                    _TimeSinceLastPoll = 0f;
                    return ETaskStatus.RUNNING;
                }
                else
                {
                    board.SetValue("HasMoveToLocation", false);
                    _TimeSinceLastPoll = 0f;
                    controller.StopMovement();
                    return ETaskStatus.SUCCESS;
                }


            }
        }

        return ETaskStatus.RUNNING;
    }
}