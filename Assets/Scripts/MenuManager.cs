using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Escape")]
    public GameObject escapeObject;
    public GameObject settings;
    public bool isPaused;
    public AudioClip clickSound;
    public AudioClip deathscreenSound;
    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            escapeObject.SetActive(false);
            settings.SetActive(false);
        }

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            Time.timeScale = 1;
            float musicVolume = 0.3f;
            AudioManagerScript.Instance.PlaySound2D(deathscreenSound, musicVolume, loop: true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Input.GetButtonDown("Escape")) 
            { 
                isPaused = !isPaused; 
                Escape();
            }  
        }
        if (player.endGame)
            Finished();
    }
    public void Finished()
    {
        player.endGameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        player.enabled = false;
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
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        escapeObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        player.enabled = true;
    }
    public void TryAgain()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        SceneManager.LoadScene("Game");
    }
    public void MainMenu()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        SceneManager.LoadScene("Menu");
    }

    public void Settings()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        escapeObject.SetActive(false);
        settings.SetActive(true);
    }

    public void Mute(float volume)
    {
        AudioListener.volume = 0.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }
    public void SetMaxVolume(float volume)
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        AudioListener.volume = 1.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetVolume(float volume)
    {
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
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        escapeObject.SetActive(true);
        settings.SetActive(false);
    }
    public void Click()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
    }
    public void EndGame()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        Application.Quit();
    }
}