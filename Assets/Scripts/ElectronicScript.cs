using UnityEngine;
using UnityEngine.Serialization;

public class RadioScript : MonoBehaviour, IInteractable
{
    public AudioSource source;
    [FormerlySerializedAs("radioClip")] public AudioClip soundClip;
    public bool isOn;
    public GameObject tvGlass;
    public Light tvLight;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.CompareTag("TV"))
        {
            tvLight.enabled = false;
            tvGlass.SetActive(false);
        }
    }
    public void RadioMusic()
    {
        if (isOn)
        {
            source = AudioManagerScript.Instance.PlaySound3D(soundClip, transform.position, volume: 1.0f, loop: true);
            if (gameObject.CompareTag("TV"))
            {
                tvLight.enabled = true;
                tvGlass.SetActive(true);
            }
        }
        else
        {
           if (source != null)
               Destroy(source);
           if (gameObject.CompareTag("TV"))
           {
               tvLight.enabled = false;
               tvGlass.SetActive(false);
           }
        }
    }

    public void Interact()
    {
        isOn = !isOn;
        RadioMusic();
    }
}
