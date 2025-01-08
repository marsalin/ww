using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript Instance;
    public Player player;
    [Header("Breath")]
    public AudioClip exhale;
    public AudioClip breathWalk;
    public AudioSource breathingSource;
    public AudioSource exhaleSource;
    public GameObject exhaleSound;
    
    [Header("Footsteps")]
    public AudioSource footstepsSource;
    public bool playedExhaleSound;
    public bool playedBreathingSound;
    
    [Header("Simple")]
    public AudioSource simpleSource;
    public AudioClip clickSound, clickSound2, clickSound3, clickSound4, clickSound5;
    
    
    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (exhaleSource == null)
        {
            exhaleSound = new GameObject("ExhaleSound");
            exhaleSource = exhaleSound.AddComponent<AudioSource>();
        }
        if (footstepsSource == null)
        {
            GameObject footstepsSound = new GameObject("FootstepsSound");
            footstepsSource = gameObject.AddComponent<AudioSource>();
        }
        if (breathingSource == null)
        {
            GameObject breathingSound = new GameObject("BreathingSound");
            breathingSource = breathingSound.AddComponent<AudioSource>();
            breathingSource.clip = breathWalk;
        }

        if (simpleSource == null)
        {
            GameObject simpleSound = new GameObject("SimpleSound");
            simpleSource = simpleSound.AddComponent<AudioSource>();
        }
    }
    
    public void Exhale()
    {
        playedExhaleSound = true;
        exhaleSource.clip = exhale;
        exhaleSource.Play();
    }

    public void BreathSound(AudioClip breathingClip)
    {
        playedBreathingSound = true;
        if (breathingSource.isPlaying)
            breathingSource.Stop();
        breathingSource.clip = breathingClip;
        breathingSource.Play();
        breathingSource.volume = 0.5f;
        breathingSource.loop = true;
    }
    public void StopBreathSound()
    {
        playedBreathingSound = false;
        breathingSource.Stop();
    }

    public void FootstepSound(AudioClip footstepClip, AudioClip footstepRugClip)
    {
        float sphereCastRadius = 0.8f;
        float sphereCastRange = 3.0f;
        Vector3 sphereCastCenter = new Vector3(player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z);
        RaycastHit hit;
        if (Physics.SphereCast(sphereCastCenter, sphereCastRadius, -player.transform.up, out hit, sphereCastRange))
        {
            if (hit.collider.CompareTag("Rug"))
                footstepsSource.clip = footstepRugClip;
            else
                footstepsSource.clip = footstepClip;
        
            footstepsSource.Play();
        }
    }
    public void SimpleSound(AudioClip simpleClip)
    {
        simpleSource.clip = simpleClip;
        simpleSource.Play();
    }

    public void PlaySound(int choice)
    {
        GameObject clickSoundObject = new GameObject("ClickSound");
        AudioSource clickSource = clickSoundObject.AddComponent<AudioSource>();
        if (choice == 1)
            clickSource.clip = clickSound;
        else if (choice == 2)
            clickSource.clip = clickSound2;
        else if (choice == 3)
            clickSource.clip = clickSound3;
        else if (choice == 4)
            clickSource.clip = clickSound4;
        else if (choice == 5)
            clickSource.clip = clickSound5;
        clickSource.Play();
        Destroy(clickSoundObject, clickSound.length/clickSource.pitch);
        DontDestroyOnLoad(clickSoundObject);
    }
}