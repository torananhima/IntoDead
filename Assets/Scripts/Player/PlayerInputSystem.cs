
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode
{
    Toggle,
    Hold
}
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputSystem : MonoBehaviour
{
    // Continuous inputs
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    // Button events: true = pressed, false = released
    public event Action<bool> InputOnSprint;
    public event Action<bool> InputOnCrouch;
    public event Action<bool> InputOnUse;
    public event Action<bool> InputOnInteract;
    public event Action<bool> InputOnDrop;
    public event Action<bool> InputOnReload;
    public event Action<bool> InputOnPause;

    private PlayerInput playerInput;
    private bool controlsDisabled;

    private readonly Dictionary<string, bool> buttonStates =
        new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            { "sprint", false },
            { "crouch", false },
            { "use", false },
            { "interact", false },
            { "drop", false },
            { "reload", false },
            { "pause", false },
            { "esc", false }
        };

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= OnActionTriggered;

        ResetInputs();
    }

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (controlsDisabled)
            return;

        string actionName = context.action.name;

        switch (actionName.ToLowerInvariant())
        {
            case "move":
                MoveInput = context.ReadValue<Vector2>();
                break;

            case "look":
                LookInput = context.ReadValue<Vector2>();
                break;

            case "sprint":
            case "crouch":
            case "use":
            case "interact":
            case "drop":
            case "reload":
            case "pause":
            case "esc":
                HandleButtonInput(context);
                break;
        }
    }

    private void HandleButtonInput(
        InputAction.CallbackContext context)
    {
        bool pressed;

        switch (context.phase)
        {
            case InputActionPhase.Performed:
                pressed = true;
                break;

            case InputActionPhase.Canceled:
                pressed = false;
                break;

            default:
                return;
        }

        string actionName = context.action.name;

        if (!buttonStates.TryGetValue(actionName, out bool previousState))
            return;

        // Emit events only when the state actually changes.
        if (previousState == pressed)
            return;

        buttonStates[actionName] = pressed;

        switch (actionName.ToLowerInvariant())
        {
            case "sprint":
                Debug.Log("SprintInput invoke");
                InputOnSprint?.Invoke(pressed);
                break;

            case "crouch":
                Debug.Log("CrouchInput invoke");
                InputOnCrouch?.Invoke(pressed);
                break;

            case "use":
                Debug.Log("UseInput invoke");
                InputOnUse?.Invoke(pressed);
                break;

            case "interact":
                Debug.Log("InteractInput invoke");
                InputOnInteract?.Invoke(pressed);
                break;

            case "drop":
                Debug.Log("DropInput invoke");
                InputOnDrop?.Invoke(pressed);
                break;

            case "reload":
                InputOnReload?.Invoke(pressed);
                break;

            case "pause":
            case "esc":
                InputOnPause?.Invoke(pressed);
                break;
        }
    }

    public void SetControlsEnabled(bool enabled)
    {
        if (controlsDisabled == !enabled)
            return;

        controlsDisabled = !enabled;

        if (controlsDisabled)
            ResetInputs();
    }

    public void OnDead()
    {
        SetControlsEnabled(false);
    }

    public void OnGameOver()
    {
        SetControlsEnabled(false);
    }

    private void ResetInputs()
    {
        MoveInput = Vector2.zero;
        LookInput = Vector2.zero;

        foreach (string actionName in new List<string>(buttonStates.Keys))
        {
            if (!buttonStates[actionName])
                continue;

            buttonStates[actionName] = false;

            switch (actionName.ToLowerInvariant())
            {
                case "sprint":
                    InputOnSprint?.Invoke(false);
                    break;

                case "crouch":
                    InputOnCrouch?.Invoke(false);
                    break;

                case "use":
                    InputOnUse?.Invoke(false);
                    break;

                case "interact":
                    InputOnInteract?.Invoke(false);
                    break;

                case "drop":
                    InputOnDrop?.Invoke(false);
                    break;

                case "reload":
                    InputOnReload?.Invoke(false);
                    break;

                case "pause":
                case "esc":
                    InputOnPause?.Invoke(false);
                    break;
            }
        }
    }
}
