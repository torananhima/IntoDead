
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
    public Vector2 WorldPoint { get; private set; }
    public Vector2 MousePos { get; private set; }

    // Button events: true = pressed, false = released
    public event Action<bool> InputOnSprint;
    public event Action<bool> InputOnCrouch;
    public event Action InputOnUse;
    public event Action InputOnInteract;
    public event Action InputOnDrop;
    public event Action InputOnReload;
    public event Action<int> InputOnChangeSlot;
    public event Action InputOnPause;

    private Vector2 halfScreenResolution;

    private PlayerInput playerInput;
    private bool controlsDisabled;
    private bool lastUsePressed;
    private readonly Dictionary<string, bool> buttonStates =
        new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            { "sprint", false },
            { "crouch", false },
            { "use", false },
            { "interact", false },
            { "drop", false },
            { "reload", false },
            { "slot1", false },
            { "slot2", false },
            { "slot3", false },
            { "slot4", false },
            { "pause", false },
            { "esc", false }
        };

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        halfScreenResolution = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
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
                Vector2 newPos = Mouse.current.position.ReadValue();
                newPos.x -= halfScreenResolution.x;
                newPos.y -= halfScreenResolution.y;
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(MoveInput);
                MousePos = newPos;
                WorldPoint = worldPoint;
                break;

            case "sprint":
            case "crouch":
            case "use":
            case "interact":
            case "drop":
            case "reload":
            case "pause":
            case "slot1":
            case "slot2":
            case "slot3":
            case "slot4":
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
                if(lastUsePressed == false && pressed){
                    InputOnUse?.Invoke();
                }
                break;

            case "interact":
                Debug.Log("InteractInput invoke");
                InputOnInteract?.Invoke();
                break;

            case "drop":
                Debug.Log("DropInput invoke");
                InputOnDrop?.Invoke();
                break;

            case "reload":
                InputOnReload?.Invoke();
                break;
            case "slot1":
                ChangeSlot(previousState, pressed, 1);
                break;
            case "slot2":
                ChangeSlot(previousState, pressed, 2);
                break;
            case "slot3":
                ChangeSlot(previousState, pressed, 3);
                break;
            case "slot4":
                ChangeSlot(previousState, pressed, 4);
                break;
            case "pause":
            case "esc":
                InputOnPause?.Invoke();
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
        MousePos = Vector2.zero;

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
                    InputOnUse?.Invoke();
                    break;

                case "interact":
                    InputOnInteract?.Invoke();
                    break;

                case "drop":
                    InputOnDrop?.Invoke();
                    break;

                case "reload":
                    InputOnReload?.Invoke();
                    break;
                case "slot1":
                case "slot2":
                case "slot3":
                case "slot4":
                    InputOnChangeSlot?.Invoke(0);
                    break;
                case "pause":
                case "esc":
                    InputOnPause?.Invoke();
                    break;
            }
        }
    }

    private void ChangeSlot(bool previousPressed, bool isPressed, int slot)
    {
        if (previousPressed == false && isPressed == true)
        {
            InputOnChangeSlot?.Invoke(slot - 1);
        }
    }
}
