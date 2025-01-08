using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menue;
    public GameObject play;
    public GameObject settings;
    public Slider volumeSlider;
    [FormerlySerializedAs("endGame")] public GameObject escapeMenu;
    private LoadingScreen loadingScreenScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settings.SetActive(false);
        escapeMenu.SetActive(false);
        play.SetActive(false);
        
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        AudioListener.volume = savedVolume;
        if (volumeSlider != null) 
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Escape();
    }

    public void StartGame()
    {
        play.SetActive(true);
    }

    public void PlayEasy()
    {
        GameManagerInstance.Instance.height = 15;
        GameManagerInstance.Instance.width = 15;
        SceneManager.LoadScene("Game");
    }
    public void PlayMedium()
    {
        GameManagerInstance.Instance.height = 20;
        GameManagerInstance.Instance.width = 20;
        SceneManager.LoadScene("Game");
    }
    public void PlayHard()
    {
        GameManagerInstance.Instance.height = 25;
        GameManagerInstance.Instance.width = 25;
        SceneManager.LoadScene("Game");
    }
    
    public void Settings()
    {
        menue.SetActive(false);
        settings.SetActive(true);
        play.SetActive(false);
    }
   
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetMaxVolume(float volume)
    {
        AudioListener.volume = 1.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetMinVolume(float volume)
    {
        AudioListener.volume = 0.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void Escape()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menue.activeSelf && !escapeMenu.activeSelf)
            escapeMenu.SetActive(true);
        else if (Input.GetKeyDown(KeyCode.Escape) && (settings.activeSelf || escapeMenu.activeSelf))
        {
            Default();
        }
    }

    public void Default()
    {
        menue.SetActive(true);
        settings.SetActive(false);
        escapeMenu.SetActive(false);
        play.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
