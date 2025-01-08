using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
     
    public Slider percentageSlider;
    public TMP_Text percentageText;
    public TMP_Text loadingText;

    void Awake()
    {
        Instance = this;
    }

    public void SetPercentage(float percentage, string text = "Doing something...")
    {
        percentageSlider.value = percentage;
        percentageText.text = $"{percentage}%";
        loadingText.text = $"{text}";
    }

    public void CloseLoadingScreen()
    {
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }
}