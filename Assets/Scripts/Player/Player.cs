using System.Collections;
using UnityEngine;


public class Player : Character
{
    [Header("Noise")]
    [SerializeField] private float noiseProgressRatio = 0.1f;
    [SerializeField] private float noiseRangeRatio = 0.2f;
    private float moveNoiseProgress;

    public PlayerInputSystem _input { get; private set; }
    public PlayerLineOfSight _los { get; private set; }
    PlayerMovement _movement;

    private void Awake()
    {
        _input = GetComponent<PlayerInputSystem>();
        _los = GetComponent<PlayerLineOfSight>();
        _movement = GetComponent<PlayerMovement>();
        ChangeMovement(MovementType.Walk);

        StartCoroutine(HandleMovement());
    }
    private void Update()
    {
        if (_movement)
        {

        }
        if(moveNoiseProgress >= 1f)
        {
            moveNoiseProgress = 0;
            NoiseSystem.NoiseOccur(NoiseSource.Player, transform.position, moveSpeed * noiseRangeRatio);
        }
    }


    IEnumerator HandleMovement()
    {
        if (!_movement) yield return null;
        float timer = 0;
        while (true)
        {
            if (_input.MoveInput != Vector2.zero) moveNoiseProgress += moveSpeed * noiseProgressRatio * Time.deltaTime;
            _movement.Move(_input.MoveInput, moveSpeed);
            if(_input.MoveInput == Vector2.zero)
            {
                timer += Time.deltaTime;
                if(timer >= 0.1f)
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
}
