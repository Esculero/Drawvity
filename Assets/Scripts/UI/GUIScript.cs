using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIScript : MonoBehaviour
{
    [SerializeField]
    private RectTransform drawingArea;

    [SerializeField]
    private DrawManager drawManager;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject failMenu;

    [SerializeField]
    private GameObject winMenu;

    [SerializeField]
    private Image penDisplay;

    [SerializeField]
    private Image inkColorDisplay;
    private int fullHeight = 280;

    [SerializeField]
    private Sprite[] penSprites;

    [SerializeField]
    private Color[] inkColors;

    private int currentPenIndex = 0;

    private void Start()
    {
        UpdateInkDisplay(0, 100f);
        drawManager = GameObject.Find("DrawManager").GetComponent<DrawManager>();
        drawManager.OnInkChanged += UpdateInkDisplay;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.GamePaused += OnGamePaused;
        gameManager.GameResumed += OnGameResumed;

        gameManager.LevelFailed += OnLevelFailed;
        gameManager.LevelWon += OnLevelWin;
    }

    public void ToggleRobot()
    {
        gameManager.ToggleRobot();
    }

    public void ToggleGamePause()
    {
        gameManager.ToggleGamePause();
    }

    public void OnGamePaused()
    {
        pauseMenu.SetActive(true);
    }

    public void OnGameResumed()
    {
        pauseMenu.SetActive(false);
    }

    public void OnLevelFailed()
    {
        failMenu.SetActive(true);
    }

    public void OnLevelWin()
    {
        winMenu.SetActive(true);
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void UpdateInkDisplay(int penIndex, float inkAmount)
    {
        if (penIndex < 0 || penIndex >= penSprites.Length || penIndex >= inkColors.Length)
            return;

        currentPenIndex = penIndex;

        penDisplay.sprite = penSprites[currentPenIndex];

        inkColorDisplay.color = inkColors[currentPenIndex];

        RectTransform rt = inkColorDisplay.rectTransform;
        float newHeight = fullHeight * inkAmount / 100;
        newHeight = Mathf.Clamp(newHeight, 0.1f, fullHeight); // to avoid destroying the rect
        // update rt rect
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
    }

    public void NextPen()
    {
        currentPenIndex = (currentPenIndex + 1) % penSprites.Length;
        drawManager.ChangeInk(currentPenIndex);
    }

    public void PreviousPen()
    {
        currentPenIndex = (currentPenIndex - 1 + penSprites.Length) % penSprites.Length;
        drawManager.ChangeInk(currentPenIndex);
    }

    public bool IsMouseInDrawingArea(Vector2 mousePos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(drawingArea, mousePos);
    }
}
