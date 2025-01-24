using System.Collections;
using TMPro;
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
    public AudioClip menuMusic;
    public AudioClip enemySound;
    public Animator animator;
    public RuntimePlatform platformToDisableOn;
    
    [Header("Jumpscare")]
    [FormerlySerializedAs("jumpScareObject")] public GameObject jumpScarePrefab;
    public TMP_Text jumpscareText;
    public Button jumpscareButton;
    public float timer = 2.0f;
    public float jumpscareTimer = 10.0f;
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
        float musicVolume = 0.3f;
        AudioManagerScript.Instance.PlaySound2D(menuMusic, musicVolume, loop: true);
    }

    // Update is called once per frame
    void Update()
    {
        Escape();
    }

    public void DoJumpScare()
    {
        StartCoroutine(JumpScare());
    }
    public IEnumerator JumpScare()
    {
        jumpscareText.text = "Nice job!";
        jumpscareButton.interactable = false;
        while (jumpscareTimer > 0)
        {
            jumpscareTimer -= Time.deltaTime;
            yield return null;
        }
        Instantiate(jumpScarePrefab);
        jumpscareText.text = "Click here";
        jumpscareButton.interactable = true;
        jumpscareTimer = 2.5f;
    }
    IEnumerator Animate()
    {
        animator.SetTrigger("Animate");
        yield return new WaitForSeconds(1.0f);
        AudioManagerScript.Instance.PlaySound2D(enemySound);
    }

    public void StartAnimation()
    {
        StartCoroutine(Animate());
    }
    
    public void StartGame()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        play.SetActive(true);
    }

    public void PlayEasy()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
        GameManagerInstance.Instance.size = 10;
        GameManagerInstance.Instance.level = "easy";
        GameManagerInstance.Instance.minRange = 30.0f;
        GameManagerInstance.Instance.maxRange = 50.0f;
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
        GameManagerInstance.Instance.size = 15;
        GameManagerInstance.Instance.level = "medium";
        GameManagerInstance.Instance.minRange = 20.0f;
        GameManagerInstance.Instance.maxRange = 50.0f;
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
        GameManagerInstance.Instance.size = 18;
        GameManagerInstance.Instance.level = "hard";
        GameManagerInstance.Instance.minRange = 20.0f;
        GameManagerInstance.Instance.maxRange = 40.0f;
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
        volumeSlider.value = 1.0f;
    }

    public void SetMinVolume(float volume)
    {
        AudioListener.volume = 0.0f;
        PlayerPrefs.SetFloat("GameVolume", volume);
        volumeSlider.value = 0.0f;
    }

    public void Escape()
    {
        if (Input.GetButtonDown("Escape"))
        {
            if (menue.activeSelf && !escapeMenu.activeSelf && Application.platform != platformToDisableOn)
                escapeMenu.SetActive(true);
            else if ((settings.activeSelf || escapeMenu.activeSelf))
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

    public void Click()
    {
        AudioManagerScript.Instance.PlaySound2D(clickSound);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
