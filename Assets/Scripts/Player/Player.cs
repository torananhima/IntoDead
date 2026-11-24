using GrozaGames.TopDownVision.MonoBehaviours;
using System;
using System.Collections;
using UnityEngine;


public class Player : Character
{
    [Header("UI")]
    [SerializeField] private Sprite walkSprite;
    [SerializeField] private Sprite sprintSprite;
    [SerializeField] private Sprite crouchSprite;
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100;

    private float currentStamina = 100;
    [Tooltip("Stamina recovery delay (second)")]
    [SerializeField] private float recoveryDelay = 0.5f;
    [Tooltip("Stamina recovery per second")]
    [SerializeField] private float staminaRecoveryRate = 20;
    [Tooltip("Stamina drained when running per second")]
    [SerializeField] private float sprintDrainRate = 33;
    public float _currentStamina
    {
        get { return  _currentStamina; }
    }
    [Header("Noise")]
    [Min(0)][SerializeField] private float noiseProgressRatio = 0.1f;
    [Min(0)][SerializeField] private float noiseRangeRatio = 1f;
    [Min(0)][SerializeField] private float walkMultRatio = 1f;
    [Min(0)][SerializeField] private float sprintMultRatio = 1f;
    [Min(0)][SerializeField] private float crouchMultRatio = 1f;

    private float moveNoiseProgress;

    public PlayerInputSystem _input { get; private set; }
    public VisionOrigin _los { get; private set; }
    public PlayerInventory _inventory { get; private set; }
    public PlayerInteract _interact { get; private set; }
    public PlayerMovement _movement { get; private set; }
    public ItemUseHandler _itemHandler { get; private set; }

    private void Awake()
    {
        _input = GetComponent<PlayerInputSystem>();
        _los = GetComponent<VisionOrigin>();
        _inventory = GetComponent<PlayerInventory>();
        _interact = GetComponent<PlayerInteract>();
        _movement = GetComponent<PlayerMovement>();
        _itemHandler = GetComponent<ItemUseHandler>();
    }

    protected override void Start()
    {
        base.Start();
        ChangeMovement(MovementType.Walk);

        StartCoroutine(HandleMovement());

        StartCoroutine(HandleStamina());
    }

    private void OnEnable()
    {
        GameManager.OnPaused += OnPause;
        GameManager.OnUnPaused += OnUnPause;
    }

    private void OnDisable()
    {
        GameManager.OnPaused -= OnPause;
        GameManager.OnUnPaused -= OnUnPause;
    }
    protected override void Update()
    {
        base.Update();
        if (moveNoiseProgress >= 1f)
        {
            moveNoiseProgress = 0;
            NoiseSystem.GetInstance().NoiseOccur(NoiseSource.Player, transform.position, moveSpeed * GetNoiseRnage(movementState));
            Debug.Log("Noise occur");
        }
        currentVelocity = _movement._controller.velocity.magnitude;
    }

    public override void TakeDamage(int damageTaken)
    {
        base.TakeDamage(damageTaken);
        UIManager.instance.UpdateHpBar(hp, maxHp);
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        UIManager.instance.UpdateHpBar(hp, maxHp);
    }
    IEnumerator HandleMovement()
    {
        if (!_movement) yield return null;
        if (CanMove()) yield return null;
        //float timer = 0;
        while (true)
        {
            if (_input.MoveInput != Vector2.zero) moveNoiseProgress += moveSpeed * noiseProgressRatio * Time.deltaTime;
            _movement.Move(_input.MoveInput, moveSpeed);
            _movement.Look(_input.MousePos);

            //if (_input.MoveInput == Vector2.zero)
            //{
            //    timer += Time.deltaTime;
            //    if (timer >= 0.1f)
            //    {
            //        _movement.ResetMovementState();
            //    }
            //}
            //else
            //{
            //    timer = 0;
            //}
            yield return null;
        }
    }

    private IEnumerator HandleStamina()
    {
        float recoveryTimer = 0;
        while (true)
        {
            if(movementState == MovementType.Sprint && currentVelocity != 0)
            {
                currentStamina -= Time.deltaTime * sprintDrainRate;
                recoveryTimer = recoveryDelay;

                if(currentStamina <= 0)
                {
                    ChangeMovement(MovementType.Walk);
                    currentStamina = 0;
                }
                UIManager.instance.UpdateStaminaBar(currentStamina, maxStamina);
            }
            if(recoveryTimer > 0)
            {
                recoveryTimer -= Time.deltaTime;
            }
            else
            {
                if (currentStamina < maxStamina)
                {
                    currentStamina += Time.deltaTime * staminaRecoveryRate;
                }
                else
                {
                    currentStamina = maxStamina;
                }
                UIManager.instance.UpdateStaminaBar(currentStamina, maxStamina);
            }
            yield return null;
        }
    }

    private float GetNoiseRnage(MovementType movementType)
    {
        switch (movementType)
        {
            case MovementType.Walk:
                return noiseRangeRatio * walkMultRatio;
            case MovementType.Sprint:
                return noiseRangeRatio * sprintMultRatio;
            case MovementType.Crouch:
                return noiseRangeRatio * crouchMultRatio;
            default: return noiseRangeRatio;
        }
    }

    private bool CanMove()
    {
        if (_itemHandler.isOnAttackCooldown)
        {
            return false;
        }
        return true;
    }
    public override void ChangeMovement(MovementType newMovement)
    {
        switch (newMovement)
        {
            case MovementType.Walk:
                newSpeed = walkSpeed;
                movementState = MovementType.Walk;
                UIManager.instance.ChangeMovementState(walkSprite);
                break;
            case MovementType.Sprint:
                newSpeed = runSpeed;
                movementState = MovementType.Sprint;
                UIManager.instance.ChangeMovementState(sprintSprite);
                break;
            case MovementType.Crouch:
                newSpeed = crouchSpeed;
                movementState = MovementType.Crouch;
                UIManager.instance.ChangeMovementState(crouchSprite);
                break;
        }
    }

    private void OnPause()
    {
        ChangeInput(false);
    }
    private void OnUnPause()
    {
        ChangeInput(true);
    }
    public void ChangeInput(bool canInput)
    {
        if (_input == null) return;
        
        _input.enabled = canInput;
    }
}