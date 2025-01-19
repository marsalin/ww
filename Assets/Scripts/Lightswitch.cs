using UnityEngine;

public class Lightswitch : MonoBehaviour, IInteractable
{
    public bool lightOn;
    public AudioClip lightSoundOn;
    public AudioClip lightSoundOff;
 
    public void Interact()
    {
        lightOn = !lightOn;
        if (lightOn)
            AudioManagerScript.Instance.PlaySound3D(lightSoundOn, transform.position);
        else
            AudioManagerScript.Instance.PlaySound3D(lightSoundOff, transform.position);
    }
}