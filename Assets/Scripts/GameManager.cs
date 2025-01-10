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
    public GameObject easyDescription, mediumDescription, hardDescription;
    public AudioClip clickSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settings.SetActive(false);
        escapeMenu.SetActive(false);
        play.SetActive(false);
        easyDescription.SetActive(false);
        mediumDescription.SetActive(false);
        hardDescription.SetActive(false);
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
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        GameManagerInstance.Instance.size = 10;
        SceneManager.LoadScene("Game");
    }

    public void HoverOverEasy()
    {
        easyDescription.SetActive(true);
    }
    public void HoverExitEasy()
    {
        easyDescription.SetActive(false);
    }
    public void PlayMedium()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        GameManagerInstance.Instance.size = 20;
        SceneManager.LoadScene("Game");
    }

    public void HoverOverMedium()
    {
        mediumDescription.SetActive(true);
    }

    public void HoverExitMedium()
    {
        mediumDescription.SetActive(false);
    }
    public void PlayHard()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        GameManagerInstance.Instance.size = 25;
        SceneManager.LoadScene("Game");
    }

    public void HoverOverHard()
    {
        hardDescription.SetActive(true);
    }

    public void HoverExitHard()
    {
        hardDescription.SetActive(false);
    }
    
    public void Settings()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
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
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        AudioListener.volume = 1.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void SetMinVolume(float volume)
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
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
        easyDescription.SetActive(false);
        mediumDescription.SetActive(false);
        hardDescription.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
