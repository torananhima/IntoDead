using UnityEngine;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Setting")]
    [SerializeField] private InputMode sprintInputMode = InputMode.Toggle;
    [SerializeField] private InputMode crouchInputMode = InputMode.Toggle;

    Player player;
    CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (player == null) player = GetComponent<Player>();

        player._input.InputOnSprint += OnSprintInput;
        player._input.InputOnCrouch += OnCrouchInput;
    }
    private void OnDisable()
    {
        if (player._input == null)
            return;

        player._input.InputOnSprint -= OnSprintInput;
        player._input.InputOnCrouch -= OnCrouchInput;
    }

    private void OnSprintInput(bool pressed)
    {
        Debug.Log("SprintInput receive");
        if(sprintInputMode == InputMode.Hold)
        {
            player.isSprinting = pressed;
        }
        if (sprintInputMode == InputMode.Toggle && pressed)
        {
            player.isSprinting = !player.isSprinting;
        }

        if (player.isSprinting)
        {
            player.isCrouching = false;
            player.ChangeMovement(MovementType.Sprint);
        }
        else
        {
            player.ChangeMovement(MovementType.Walk);
        }
    }
    private void OnCrouchInput(bool pressed)
    {
        Debug.Log("CrouchInput receive");
        if (crouchInputMode == InputMode.Hold)
        {
            player.isCrouching = pressed;
        }
        if (crouchInputMode == InputMode.Toggle && pressed)
        {
            player.isCrouching = !player.isCrouching;
        }

        if(player.isCrouching)
        {
            player.isSprinting = false;
            player.ChangeMovement(MovementType.Crouch);
        }
        else
        {
            player.ChangeMovement(MovementType.Walk);
        }
    }

    public void Move(Vector2 input, float moveSpeed)
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        _controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    public void ResetMovementState()
    {
        player.isSprinting = false;
        player.ChangeMovement(MovementType.Walk);
    }
}
