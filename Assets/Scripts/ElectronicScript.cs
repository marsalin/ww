using UnityEngine;
using UnityEngine.Serialization;

public class RadioScript : MonoBehaviour, IInteractable
{
    public AudioSource musicSource;
    [FormerlySerializedAs("radioClip")] public AudioClip soundClip;
    public bool isOn;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject musicObj = new GameObject("RadioMusic");
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.clip = soundClip;
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
