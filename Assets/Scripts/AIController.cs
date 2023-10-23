using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AIController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform TargetTransform;
    [SerializeField] private float _MovementSpeed;
    [SerializeField] private float _NextWayPointDistance;
    private Path _Path;
    private int _CurrentWaypoint = 0;
    private bool _HasReachedPath = false;

    [Header("AI")] 
    [SerializeField] private BehaviorTree _Tree;
    private Blackboard _Blackboard;
    
    [Header("Components")]
    [SerializeField] private SpriteRenderer _Render;
    [SerializeField] private Seeker _Seeker;
    [SerializeField] private Rigidbody2D _RBody;
    [SerializeField] private Animator _Anim;

    private void Awake()
    {
        if (!_Seeker)
            TryGetComponent(out _Seeker);
        if (_RBody)
            TryGetComponent(out _RBody);
    }

    protected virtual void Start()
    {
        CreateBlackboard();
        _Tree.Initialize(_Blackboard);

        _Blackboard.SetValue("MoveToLocation", new Vector2(-8, -4.51f));
        _Blackboard.SetValue("HasMoveToLocation", true);
    }

    public void UpdatePath(Vector2 position)
    {
        if(_Seeker.IsDone())
            _Seeker.StartPath(_RBody.position, position, OnPathComplete);
        
        _Anim.SetBool("IsMoving", true);
    }

    public void StopMovement()
    {
        _Path = null;
        _Anim.SetBool("IsMoving", false);
        _RBody.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private bool HasReachedPath()
    {
        if (_Path == null)
            return true;

        if (_CurrentWaypoint >= _Path.vectorPath.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandleMovement()
    {
        if (HasReachedPath())
            return;

        Vector2 direction = ((Vector2)_Path.vectorPath[_CurrentWaypoint] - _RBody.position).normalized;
        Vector2 force = direction * (_MovementSpeed * Time.deltaTime);

        _RBody.velocity = force;

        float distance = Vector2.Distance(_RBody.position, _Path.vectorPath[_CurrentWaypoint]);
        if (distance < _NextWayPointDistance)
            _CurrentWaypoint++;

        if (force.x > 0)
        {
            _Render.flipX = false;
        }
        else if(force.x < 0)
        {
            _Render.flipX = true;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _Path = p;
            _CurrentWaypoint = 0;
        }
    }

    protected virtual void CreateBlackboard()
    {
        _Blackboard = new Blackboard();
        _Blackboard.SetValue<GameObject>("Self", this.gameObject);
        _Blackboard.SetValue<GameObject>("Target", null);
        _Blackboard.SetValue("HasMoveToLocation", false);
        _Blackboard.SetValue("MoveToLocation", Vector2.zero);

    }
}
