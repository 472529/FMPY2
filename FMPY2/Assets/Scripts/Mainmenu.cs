using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
public class Mainmenu : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    private bool playButtonClicked = false;
    public bool inverted = false;

    public TextMeshProUGUI invertedText;
    public GameObject settings;
    public GameObject menu;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (playButtonClicked == true)
        {
            LoadNextLevel();
            playButtonClicked = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void WaveMode()
    {
        SceneManager.LoadScene(2);
    }

    public void Settings()
    {
        menu.SetActive(false);
        settings.SetActive(true);

    }

    public void SettingsBack()
    {
        
        settings.SetActive(false);
        menu.SetActive(true);
    }

    public void Inverted()
    {
        inverted = !inverted;
        if (inverted)
        {
            invertedText.faceColor = new Color32(255, 128, 0, 255);
        }
        else
        {
            invertedText.faceColor = new Color32(255, 128, 255, 255);
        }
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
