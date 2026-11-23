using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Gameplay,
    Paused,
    End
}
public class GameManager : MonoBehaviour
{
    public static GameState currentGameState;
    public static Player player;
    private static GameManager instance;
    public float gameTimer;

    public static Action OnGameStart;

    public static Action OnPaused;
    public static Action OnUnPaused;

    public static Action OnGameEnd;
    [SerializeField] InputActionReference pauseInput;
    bool wasPressedPause;

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameManager();
        }
        return instance;
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        player = FindAnyObjectByType<Player>().GetComponent<Player>();

        Time.timeScale = 1;
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
    }
    private void OnEnable()
    {
        pauseInput.action.performed += TogglePause;
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        pauseInput.action.performed -= TogglePause;
    }
    public void ChangeGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                if (currentGameState == GameState.Paused) OnUnPaused?.Invoke();
                currentGameState = GameState.Gameplay;
                Time.timeScale = 1;
                break;
            case GameState.Paused:
                currentGameState = GameState.Paused;
                OnPaused?.Invoke();
                Time.timeScale = 0;
                break;
            case GameState.End:
                currentGameState = GameState.End;
                OnGameEnd?.Invoke();
                break;
            default:
                currentGameState = GameState.Gameplay;
                break;

        }
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        Time.timeScale = 1;
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            if (currentGameState == GameState.Paused)
            {
                ChangeGameState(GameState.Gameplay);
                OnUnPaused?.Invoke();
            }
            else
            {
                ChangeGameState(GameState.Paused);
                OnPaused?.Invoke();
            }
        }
    }
    public void Pause()
    {
        if (currentGameState == GameState.Paused) return;
        ChangeGameState(GameState.Paused);
        OnPaused?.Invoke();
    }
    public void UnPause()
    {
        if (currentGameState == GameState.Gameplay) return;
        ChangeGameState(GameState.Gameplay);
        OnUnPaused?.Invoke();
    }
}