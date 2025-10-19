using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // managers
    InputManager inputManager;
    // public GUIManager guiManager;
    // public LevelManager levelManager;

    // Events for game state changes
    public event Action GameStarted;
    public event Action GamePaused;
    public event Action GameResumed;
    public event Action GameEnded;

    public event Action LevelWon;
    public event Action LevelFailed;

    public event Action PauseRobotToggled;

    bool isGamePaused = false;
    bool isLevelFinished = false;

    private void Awake()
    {

    }

    void Start()
    {
        inputManager = GetComponent<InputManager>();

        inputManager.OnGamePausePressed += ToggleGamePause;      
        inputManager.OnRobotPausePressed += ToggleRobotPause;
    }

    private void OnDestroy()
    {
        inputManager.OnGamePausePressed -= ToggleGamePause;
        inputManager.OnRobotPausePressed -= ToggleRobotPause;
    }

    public void StartGame()
    {
        GameStarted?.Invoke();
    }
    public void EndGame()
    {
        GameEnded?.Invoke();
    }

    public void ToggleGamePause()
    {
        if (isLevelFinished) return;

        switch(isGamePaused)
        {
            case true:
                GameResumed?.Invoke();
                isGamePaused = false;
                break;
            case false:
                GamePaused?.Invoke();
                isGamePaused = true;
                break;
        }
    }

    public void ToggleRobot()
    {
        PauseRobotToggled?.Invoke();
    }

    public void FailLevel()
    {
        isLevelFinished = true;
        LevelFailed?.Invoke();
    }

    public void WinLevel()
    {
        isLevelFinished = true;
        LevelWon?.Invoke();
    }

    void ToggleRobotPause()
    {
        if(isLevelFinished || isGamePaused) return;

        PauseRobotToggled?.Invoke();
    }
}
