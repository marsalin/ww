using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Escape")]
    public GameObject escapeObject;
    public GameObject settings;
    public bool isPaused;

    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        escapeObject.SetActive(false);
        settings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            isPaused = !isPaused;
            Escape();
        }
    }
    
    public void Escape()
    {
        if (isPaused)
        {
            escapeObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            player.enabled = false;
        }
        else
        {
           Default();
        }
    }

    public void Continue()
    {
        AudioManagerScript.Instance.PlaySound(1);
        escapeObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        player.enabled = true;
    }
    
    public void MainMenu()
    {
        AudioManagerScript.Instance.PlaySound(1);
        SceneManager.LoadScene("Menu");
    }

    public void Settings()
    {
        AudioManagerScript.Instance.PlaySound(1);
        settings.SetActive(true);
        escapeObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void Mute(float volume)
    {
        AudioManagerScript.Instance.PlaySound(2);
        AudioListener.volume = 0.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }
    public void SetMaxVolume(float volume)
    {
        AudioManagerScript.Instance.PlaySound(3);
        AudioListener.volume = 1.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetVolume(float volume)
    {
        AudioManagerScript.Instance.PlaySound(4);
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void Default()
    {
        escapeObject.SetActive(false);
        settings.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        player.enabled = true;
        isPaused = false;
    }

    public void GoBack()
    {
        AudioManagerScript.Instance.PlaySound(1);
        escapeObject.SetActive(true);
        settings.SetActive(false);
    }

    public void TryAgain()
    {
        AudioManagerScript.Instance.PlaySound(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    

    public void EndGame()
    {
        AudioManagerScript.Instance.PlaySound(5);
        Application.Quit();
    }
}