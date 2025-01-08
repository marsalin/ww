using UnityEngine;

public class RadioScript : MonoBehaviour, IInteractable
{
    public AudioSource musicSource;
    public AudioClip radioClip;
    public bool isOn;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject musicObj = new GameObject("RadioMusic");
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.clip = radioClip;
    }

    // Update is called once per frame
    void Update()
    {
        RadioMusic();
    }

    public void RadioMusic()
    {
        if (isOn)
            musicSource.Play();
        else
            musicSource.Stop();
    }

    public void Interact()
    {
        isOn = !isOn;
    }
}
