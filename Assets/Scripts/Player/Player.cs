using GrozaGames.TopDownVision.MonoBehaviours;
using System.Collections;
using UnityEngine;


public class Player : Character
{
    private float maxStamina = 100;
    private float currentStamina = 100;
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

    private void Awake()
    {
        _input = GetComponent<PlayerInputSystem>();
        _los = GetComponent<VisionOrigin>();
        _inventory = GetComponent<PlayerInventory>();
        _interact = GetComponent<PlayerInteract>();
        _movement = GetComponent<PlayerMovement>();
        ChangeMovement(MovementType.Walk);

        StartCoroutine(HandleMovement());
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
    }

    public override void TakeDamage(int damageTaken)
    {
        base.TakeDamage(damageTaken);
        UIManager.instance.UpdateHpBar(hp, maxHp);
    }

    IEnumerator HandleMovement()
    {
        if (!_movement) yield return null;
        float timer = 0;
        while (true)
        {
            if (_input.MoveInput != Vector2.zero) moveNoiseProgress += moveSpeed * noiseProgressRatio * Time.deltaTime;
            _movement.Move(_input.MoveInput, moveSpeed);
            _movement.Look(_input.MousePos);

            if (_input.MoveInput == Vector2.zero)
            {
                timer += Time.deltaTime;
                if (timer >= 0.1f)
                {
                    _movement.ResetMovementState();
                }
            }
            else
            {
                timer = 0;
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
}
