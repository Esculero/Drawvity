using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // managers
    InputManager inputManager;
    // public GUIManager guiManager;
    // public LevelManager levelManager;

    // Events for game state changes
    public event Action OnGameStarted;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameEnded;

    public event Action OnLevelCompleted;
    public event Action OnLevelFailed;

    public event Action OnRobotPausedToggled;

    bool isGamePaused = false;

    private void Awake()
    {
        // ensure that there is only one GameManager in the scene
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        inputManager = GetComponent<InputManager>();

        inputManager.OnGamePausePressed += ToggleGamePause;      
        inputManager.OnRobotPausePressed += ToggleRobotPause;
    }

    void StartGame()
    {
        OnGameStarted?.Invoke();
    }
    void EndGame()
    {
        OnGameEnded?.Invoke();
    }

    void ToggleGamePause()
    {
        switch(isGamePaused)
        {
            case true:
                OnGameResumed?.Invoke();
                isGamePaused = false;
                break;
            case false:
                OnGamePaused?.Invoke();
                isGamePaused = true;
                break;
        }
    }

    void FailLevel()
    {
        OnLevelFailed?.Invoke();
    }

    void WinLevel()
    {
        OnLevelCompleted?.Invoke();
    }

    void ToggleRobotPause()
    {
        OnRobotPausedToggled?.Invoke();
    }
}
