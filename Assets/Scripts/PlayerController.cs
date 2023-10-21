using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float _MovementSpeed;
    [SerializeField] private float _ColliderFacingRight;
    [SerializeField] private float _ColliderFacingLeft;
    private bool _Flipped = false;
    private bool _CanMove = true;

    [Header("Gravity")] 
    [SerializeField] private Transform _GroundedCheckTransform;                     // Reference to the raycast start point        
    [SerializeField] private float _GroundRadius;                // How far from the ground the character needs to be before being grounded
    [SerializeField] private LayerMask _GroundLayer;                    // Reference to the ground layer
    [SerializeField] private float _GravityModifier = 1.0f;
    private float _Gravity = -9.81f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _AirControl;                 // How much the character can move while not grounded
    [SerializeField] private float _JumpForce;
    private bool _PerformJump;
    
    [Header("Component")] 
    [SerializeField] private Rigidbody2D _Rbody;
    [SerializeField] private Animator _Anim;
    [SerializeField] private SpriteRenderer _Render;
    [SerializeField] private CapsuleCollider2D _Collider;                       // reference to the collider

    [Header("Debug")] 
    [SerializeField] private bool DebugMode = true;
    

    private PlayerInputs _Inputs;

    private void Awake()
    {
        _Inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        _Inputs.Enable();
    }

    private void OnDisable()
    {
        _Inputs.Disable();
    }

    private void Update()
    {
        if (DebugMode)
        {
            Debug.Log(IsGrounded());
        }
        
        if(_Anim)
            _Anim.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!_CanMove)
            return;

        float movementDirection = _Inputs.Player.Move.ReadValue<float>();
        Vector2 movementInput = Vector2.zero;

        if (movementDirection != 0)
        {
            movementInput = new Vector2
            {
                x = movementDirection + (GetMovementSpeed() * Time.deltaTime),
                y = 0f
            };
        }

        if (_PerformJump)
        {
            _Rbody.velocity = Vector2.zero;
            movementInput = new Vector2
            {
                x = 0,
                y = movementInput.y + _JumpForce
            };
            if(_Anim)
                _Anim.SetTrigger("Jump");
            _PerformJump = false;
        }
        else
        {
            if (!IsGrounded())
            {
                movementInput = new Vector2
                {
                    x = movementInput.x,
                    y = _Gravity * _GravityModifier
                };
            }
            else
            {
                movementInput = new Vector2
                {
                    x = movementInput.x,
                    y = 0f
                };
            }
        }

        if (movementDirection > 0)
        {
            _Flipped = false;
            
        } else if (movementDirection < 0)
        {
            _Flipped = true;
        }

        if (_Flipped)
        {
            if (!_Render.flipX)
            {
                _Render.flipX = true;
                _Collider.offset = new Vector2(_ColliderFacingLeft, _Collider.offset.y);
            }
                
        }
        else
        {
            if (_Render.flipX)
            {
                _Render.flipX = false;
                _Collider.offset = new Vector2(_ColliderFacingRight, _Collider.offset.y);   
            }
                
        }

        if (_Anim)
            _Anim.SetBool("IsMoving", movementDirection != 0);

        if (!IsGrounded())
        {
            _Rbody.velocity += movementInput;
        }
        else
        {
            _Rbody.velocity = movementInput;
        }
    }

    public bool IsGrounded()
    {
        if(Physics2D.Raycast(_GroundedCheckTransform.position, Vector2.down, _GroundRadius, _GroundLayer))
        {
            return true;
        }
        return false;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && IsGrounded())
        {
            _PerformJump = true;
        }
    }

    private float GetMovementSpeed()
    {
        if (IsGrounded())
            return _MovementSpeed;

        return _AirControl;
    }
    
}
