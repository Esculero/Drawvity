using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions inputSystem;

    #region events

    // Events to hook into for drawing
    public event Action OnDrawStarted;
    public event Action OnDrawEnded;
    public event Action<Vector2> OnDrawPositionChanged;
    public event Action OnErasePressed;

    // Events for ink changes
    public event Action<int> OnInkTypeSelected;

    // Events for robot
    public event Action OnRobotPausePressed;

    // Event for game
    public event Action OnGamePausePressed;

    #endregion

    void Awake()
    {
        inputSystem = new();
        inputSystem.Enable();

        /// Drawing input hooks
        inputSystem.Player.Draw.performed +=
            context =>
            {
                Vector2 drawPos = context.ReadValue<Vector2>();
                OnDrawPositionChanged?.Invoke(drawPos);
            };

        inputSystem.Player.IsDrawing.started += Context =>
        {
            OnDrawStarted?.Invoke();
        };

        inputSystem.Player.IsDrawing.canceled += Context =>
        {
            OnDrawEnded?.Invoke();
        };

        inputSystem.Player.Erase.started += Context =>
        {
            OnErasePressed?.Invoke();
        };

        /// Ink type change hooks
        inputSystem.Player.Ink1.started += Context =>
        {
            OnInkTypeSelected?.Invoke(0);
        };

        inputSystem.Player.Ink2.started += Context =>
        {
            OnInkTypeSelected?.Invoke(1);
        };

        inputSystem.Player.Ink3.started += Context =>
        {
            OnInkTypeSelected?.Invoke(2);
        };

        /// Robot hook
        inputSystem.Player.PauseRobot.started += Context =>
        {
            OnRobotPausePressed?.Invoke();
        };

        /// Game hooks
        inputSystem.Player.PauseGame.started += Context =>
        {
            OnGamePausePressed?.Invoke();
        };
    }
}
