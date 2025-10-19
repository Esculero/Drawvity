using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField]
    private GameObject quitButton;

    [SerializeField]
    private GameManager gameManager;

    private SoundManager soundManager;

    bool isOptionsActive = false;
    bool isCreditsActive = false;


    void Start()
    {
        soundManager = gameManager.gameObject.GetComponent<SoundManager>();

        // if build type is WebGL, disable quit button
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region MainMenu
    public void StartGame()
    {
        Debug.Log("Start Game");
        SceneManager.LoadScene("Level0"); // load first level
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
    #endregion

    #region OptionsMenu
    public void UpdateSfxVolume(float volume)
    {
        Debug.Log("SFX Volume: " + volume);
        // save volume to SoundManager

        soundManager.SFXVolume = volume;
    }

    public void UpdateBgmVolume(float volume)
    {
        Debug.Log("BGM Volume: " + volume);
        // save volume to SoundManager

        soundManager.BGMVolume = volume;
    }
    #endregion

    public void ToggleOptions()
    {
        Debug.Log("Toggle Options");

        isOptionsActive = !isOptionsActive;
        mainMenuPanel.SetActive(!isOptionsActive);
        optionsPanel.SetActive(isOptionsActive);
    }

}