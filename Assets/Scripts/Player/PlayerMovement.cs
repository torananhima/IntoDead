using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Setting")]
    [SerializeField] private InputMode sprintInputMode = InputMode.Toggle;
    [SerializeField] private InputMode crouchInputMode = InputMode.Toggle;

    Player player;
    public CharacterController _controller { get; private set; }
    public Action OnMove;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (player == null) player = GetComponent<Player>();

        player._input.InputOnSprint += OnSprintInput;
        player._input.InputOnCrouch += OnCrouchInput;

        _controller.Move(new Vector3(0f, -0.01f, 0f));
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
        }else
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
        }else
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
        if (input != Vector2.zero) OnMove?.Invoke();
    }

    public void ResetMovementState()
    {
        player.isSprinting = false;
        player.ChangeMovement(MovementType.Walk);
    }

    public void Look(Vector2 lookInput)
    {
        Vector2 lookPos = new Vector2(lookInput.x + transform.position.x, lookInput.y + transform.position.z);
        transform.LookAt(new Vector3(lookPos.x, transform.position.y, lookPos.y));
    }
}
