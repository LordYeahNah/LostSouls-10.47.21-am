using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float _MovementSpeed;                          // Speed the character moves at
    [SerializeField] private float _ColliderFacingRight;                // Position of the collider facing right
    [SerializeField] private float _ColliderFacingLeft;                     // Position of the collider facing left
    private bool _Flipped = false;                          // If the sprite is flipped
    private bool _CanMove = true;                       // If the character can move

    [Header("Sweep Attack")] 
    [SerializeField] private float _SweepAttackCooldown;
    
    [Header("Slam Attack")]
    [SerializeField] private float _SlamAttackCooldown;
    [SerializeField] private float _GroundDistanceToAttack;                     // How far is required to be before finishing the attack
    [SerializeField] private LayerMask _SlamLayer;
    private bool _IsInSlamAttack = false;
    private bool _IsAttacking = false;                       // If the player is currently attacking

    [Header("Spin Attack")] 
    [SerializeField] private float _SpinAttackCooldown = 0.35f;
    private bool _SpinAttackEnabled = false;                        // If we are wanting to perform a spin attack
    

    [Header("Gravity")] 
    [SerializeField] private Transform _GroundedCheckTransform;                     // Reference to the raycast start point        
    [SerializeField] private float _GroundRadius;                // How far from the ground the character needs to be before being grounded
    [SerializeField] private LayerMask _GroundLayer;                    // Reference to the ground layer
    [SerializeField] private float _GravityModifier = 1.0f;
    [SerializeField] private bool _ApplyGravity = false;                    // If we are to add gravity to the movement
    private float _Gravity = -9.81f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _AirControl;                 // How much the character can move while not grounded
    [SerializeField] private float _JumpForce;
    private bool _PerformJump;                          // If we need to perform the jump
    private float _ResetGravityModifier;                       // used to store the last gravity when required to toggle between scales
    
    [Header("Component")] 
    [SerializeField] private Rigidbody2D _Rbody;
    [SerializeField] private Animator _Anim;
    [SerializeField] private SpriteRenderer _Render;
    [SerializeField] private CapsuleCollider2D _Collider;                       // reference to the collider

    [Header("Debug")] [SerializeField] private bool DebugMode = true;
    

    private PlayerInputs _Inputs;

    private void Awake()
    {
        _Inputs = new PlayerInputs();                       // Create new inputs

        
        // Attempt to get components, if haven't already
        if (!_Rbody)
            TryGetComponent(out _Rbody);
        if (!_Anim)
            TryGetComponent(out _Anim);
        if (!_Render)
            TryGetComponent(out _Render);
        if (!_Collider)
            TryGetComponent(out _Collider);
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

        if (_IsInSlamAttack)
        {
            if (Physics2D.OverlapCircle(_GroundedCheckTransform.position, 
                    _GroundDistanceToAttack, _SlamLayer))
            {
                if (_Anim)
                    _Anim.speed = 1;

                _IsInSlamAttack = false;
            }
        }
        
        // Update the grounded check on the animator
        if(_Anim)
            _Anim.SetBool("IsGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float movementDirection = _Inputs.Player.Move.ReadValue<float>();                   // Read the float value
        Vector2 movementInput = Vector2.zero;                   // define the movement input

        // Check if moving

        // If performing the jump
        if (_PerformJump)
        {
            _Rbody.velocity = Vector2.zero;                     // reset the vector velocity
            // Apply the jump force
            movementInput = new Vector2
            {
                x = 0,
                y = _JumpForce * Time.deltaTime
            };
            // Update animation
            if(_Anim)
                _Anim.SetTrigger("Jump");
            _PerformJump = false;                       // Reset the flag
        }
        else
        {
            if (!IsGrounded())
            {
                movementInput = new Vector2
                {
                    x = movementDirection * _MovementSpeed,
                    y = (_Gravity * _GravityModifier)
                };

                movementInput = movementInput * Time.deltaTime;
            }
            else
            {
                if (movementDirection != 0)
                {
                    if (_CanMove)
                    {
                        // Create the movement input
                        movementInput = new Vector2
                        {
                            x = movementDirection + (GetMovementSpeed() * Time.deltaTime),
                            y = 0f
                        };
                    }
                }
            }
        }

        HandleMovementAnimation(movementDirection);
        
        if (!IsGrounded())
        {
            _Rbody.velocity += movementInput;
        }
        else
        {
            _Rbody.velocity = movementInput;
        }
    }

    private void HandleMovementAnimation(float movementDirection)
    {
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

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (_IsAttacking)
            return;

        _IsAttacking = true;
        if(_Anim)
            _Anim.SetTrigger("Attack");
        

        if (IsGrounded())
        {
            if (!_SpinAttackEnabled)
            {
                StartCoroutine(AttackCooldown(_SweepAttackCooldown));
            }
            else
            {
                StartCoroutine(AttackCooldown(_SpinAttackCooldown));
            }
        }
        else
        {
            StartCoroutine(AttackCooldown(_SlamAttackCooldown));
        }
    }

    public void ToggleSpinAttack(InputAction.CallbackContext ctx)
    {
        _SpinAttackEnabled = ctx.performed;
        if(_Anim)
            _Anim.SetBool("AttackModifier", _SpinAttackEnabled);
    }

    private IEnumerator AttackCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _CanMove = true;
        _IsAttacking = false;
    }

    private float GetMovementSpeed()
    {
        if (IsGrounded())
            return _MovementSpeed;

        return _AirControl;
    }

    public void OnPauseSlamAnimation()
    {
        _Anim.speed = 0f;
        _IsInSlamAttack = true;
    }
}
