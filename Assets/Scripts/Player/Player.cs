using GrozaGames.TopDownVision.MonoBehaviours;
using System.Collections;
using UnityEngine;


public class Player : Character
{
    [Header("Noise")]
    [SerializeField] private float noiseProgressRatio = 0.1f;
    [SerializeField] private float noiseRangeRatio = 0.2f;
    private float moveNoiseProgress;

    [SerializeField] float pickupRange = 3f;

    public PlayerInputSystem _input { get; private set; }
    public VisionOrigin _los { get; private set; }
    public PlayerInventory _inventory { get; private set; }
    PlayerMovement _movement;

    private void Awake()
    {
        _input = GetComponent<PlayerInputSystem>();
        _los = GetComponent<VisionOrigin>();
        _inventory = GetComponent<PlayerInventory>();
        _movement = GetComponent<PlayerMovement>();
        ChangeMovement(MovementType.Walk);

        StartCoroutine(HandleMovement());
        _input.InputOnInteract += Interact;
    }

    private void OnDisable()
    {
        _input.InputOnInteract -= Interact;
    }
    private void Update()
    {
        if(moveNoiseProgress >= 1f)
        {
            moveNoiseProgress = 0;
            NoiseSystem.GetInstance().NoiseOccur(NoiseSource.Player, transform.position, moveSpeed * noiseRangeRatio);
            Debug.Log("Noise occur");
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
            _movement.Look(_input.MousePos);

            if (_input.MoveInput == Vector2.zero)
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

    private void Interact(bool isPressed)
    {
        Debug.Log("Interact input received");
        if (!isPressed) return;
        Debug.Log("Interact input pressed");

        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var collider in colliders)
        {
            DroppedItem droppedItem = collider.GetComponent<DroppedItem>();
            Debug.Log($"Found dropped item: {droppedItem?.name ?? "null"}");
            Debug.Log($"Can interact: {droppedItem?.CanInteract(gameObject) ?? false}");
            if (droppedItem != null && droppedItem.CanInteract(gameObject))
            {
                droppedItem.Interact(gameObject);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
