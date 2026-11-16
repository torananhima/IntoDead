using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

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

    public static GameManager GetInstance()
    {
        if(instance == null)
        {
            instance = new GameManager();
        }
        return instance;
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        player = FindAnyObjectByType<Player>().GetComponent<Player>();
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
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
}