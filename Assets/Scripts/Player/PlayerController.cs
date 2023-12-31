using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum ESweeperAttackType
{
    SWEEP = 0,
    SLAM = 1,
    SPIN = 2,
}

public abstract class PlayerController : MonoBehaviour
{
    [SerializeField] private EPlayerCharacter _CharacterType;
    [Header("Movement Settings")] 
    [SerializeField] protected float _MovementSpeed;                          // Speed the character moves at
    [SerializeField] protected float _ColliderFacingRight;                // Position of the collider facing right
    [SerializeField] protected float _ColliderFacingLeft;                     // Position of the collider facing left
    protected bool _Flipped = false;                          // If the sprite is flipped
    protected bool _CanMove = true;                       // If the character can move

    [Header("General Combat")]
    [SerializeField] protected float _DamagePoints;                       // Points applied when damage is dealt
    [SerializeField, Range(0f, 1f)] protected float _CriticalHitChance;
    [SerializeField] protected float _CriticalHitModifier = 1.0f;
    [SerializeField] protected float _AttackDistance;                         // How far the enemy has to be to detect attack
    [SerializeField] protected Transform _AttackCastTransform;                    // Where the attack beings
    [SerializeField] protected LayerMask _DamageMask;
    protected ESweeperAttackType _AttackType;                            // Reference to the current attack type
    protected bool _IsAttacking = false;                       // If the player is currently attacking
    protected bool _HeavyAttack = false;
    

    [Header("Gravity")] 
    [SerializeField] protected Transform _GroundedCheckTransform;                     // Reference to the raycast start point        
    [SerializeField] protected float _GroundRadius;                // How far from the ground the character needs to be before being grounded
    [SerializeField] protected LayerMask _GroundLayer;                    // Reference to the ground layer
    [SerializeField] protected float _GravityModifier = 1.0f;
    [SerializeField] protected bool _ApplyGravity = false;                    // If we are to add gravity to the movement
    protected float _Gravity = -9.81f;
    
    [Header("Jump Settings")]
    [SerializeField] protected float _AirControl;                 // How much the character can move while not grounded
    [SerializeField] protected float _JumpForce;
    protected bool _PerformJump;                          // If we need to perform the jump
    protected float _ResetGravityModifier;                       // used to store the last gravity when required to toggle between scales
    
    [Header("Component")] 
    [SerializeField] protected Rigidbody2D _Rbody;
    [SerializeField] protected Animator _Anim;
    [SerializeField] protected SpriteRenderer _Render;
    [SerializeField] protected CapsuleCollider2D _Collider;                       // reference to the collider

    [Header("Debug")] [SerializeField] protected bool DebugMode = true;

    [Header("Character Health")]
    [SerializeField] protected float _MaxHealth;
    [SerializeField] protected float _CurrentHealth;
    public float CurrentHealth => _CurrentHealth;
    public UnityEvent _CharacterTakeDamage;

    [FormerlySerializedAs("_IsInInteraction")] [Header("Interactions")] 
    public bool IsInInteraction = false;
    public IInteractable _Interactable;
    
    

    protected PlayerInputs _Inputs;

    public void SetupData(PlayerSaveData data, bool setPosition = false)
    {
        if (setPosition)
            this.transform.position = data.SavePosition;

        _CurrentHealth = data.CurrentHealth;
        if(_CurrentHealth < _MaxHealth)
            _CharacterTakeDamage?.Invoke();

    }

    protected virtual void Awake()
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

        _CurrentHealth = _MaxHealth;
    }

    protected virtual void OnEnable()
    {
        _Inputs.Enable();
    }

    protected virtual void OnDisable()
    {
        _Inputs.Disable();
    }

    protected virtual void Update()
    {
        
        // Update the grounded check on the animator
        if(_Anim)
            _Anim.SetBool("IsGrounded", IsGrounded());
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected void HandleMovement()
    {

        if (IsInInteraction)
        {
            _Rbody.velocity = Vector2.zero;
            HandleMovementAnimation(0f);
            return;
        }
        
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

    protected virtual void HandleMovementAnimation(float movementDirection)
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
            this.transform.localScale = new Vector3(-1, 1, 0);    
        }
        else
        {
            this.transform.localScale = new Vector3(1, 1, 0);
            
                
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
        if (ctx.performed)
        {
            if (IsInInteraction)
                HandleInteractionUpdate();
            else
                if(IsGrounded())
                    _PerformJump = true;

        }
    }

    protected void HandleInteractionUpdate()
    {
        if (_Interactable != null)
        {
            _Interactable.Interact();
        }
    }
    
    public virtual void Attack(InputAction.CallbackContext ctx)
    {
        if (_IsAttacking || IsInInteraction)
            return;

        _IsAttacking = true;
        if(_Anim)
            _Anim.SetTrigger("Attack");
        
        GenerateAttack();
    }

    protected abstract void GenerateAttack();

    public virtual void PerformAttack()
    {
        Vector2 direction = Vector2.zero;
        if (_Flipped)
        {
            direction = transform.TransformDirection(-Vector2.right);
        }
        else
        {
            direction = transform.TransformDirection(Vector2.right);
        }

        RaycastHit2D hit = Physics2D.Raycast(_AttackCastTransform.position, direction, _AttackDistance, _DamageMask);
        if (hit)
        {
            if(hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Hit Enemy");
                hit.collider.GetComponent<EnemyController>()?.TakeDamage(GenerateDamagePoints(), 
                    _Rbody.position, GetAttackForce());
            }
        }
    }

    protected abstract float GenerateDamagePoints();
    protected abstract float GetAttackForce();

    public void ToggleSpinAttack(InputAction.CallbackContext ctx)
    {
        _HeavyAttack = ctx.performed;
        if(_Anim)
            _Anim.SetBool("AttackModifier", _HeavyAttack);
    }

    protected IEnumerator AttackCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        AttackReset();
    }

    protected virtual void AttackReset()
    {
        _CanMove = true;
        _IsAttacking = false;
        _HeavyAttack = false;
    }

    private float GetMovementSpeed()
    {
        if (IsGrounded())
            return _MovementSpeed;

        return _AirControl;
    }

    public void TakeDamage(float damagePoints)
    {
        _CurrentHealth -= damagePoints;
        if (_CurrentHealth < 0)
        {
            // TODO: Play death animation
            _CanMove = false;
        }
        else
        {
            if(_Anim)
                _Anim.SetTrigger("TakeHit");
            
            _CharacterTakeDamage?.Invoke();
        }
    }

    public void EnterInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!IsInInteraction && _Interactable != null)
            {
                _Interactable.Interact();
                IsInInteraction = true;
            }
        }
    }

    public PlayerSaveData SavePlayerData()
    {
        return new PlayerSaveData(_CharacterType, _CurrentHealth, Vector2.zero);
    }
}
