using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Escape")]
    public GameObject escapeObject;
    public GameObject settings;
    private bool isPaused;

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
            player.canMove = false;
        }
        else
        {
            escapeObject.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            player.canMove = true;
        }
    }

    public void Continue()
    {
        escapeObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void HomeMenu()
    {
        
    }

    public void Settings()
    {
        settings.SetActive(true);
        escapeObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void Mute(float volume)
    {
        AudioListener.volume = 0.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }
    public void SetMaxVolume(float volume)
    {
        AudioListener.volume = 1.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}