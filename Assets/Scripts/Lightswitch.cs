using System;
using UnityEngine;

public class Lightswitch : MonoBehaviour, IInteractable
{
    public bool lightOn;
    public AudioClip lightSoundOn;
    public AudioClip lightSoundOff;
    public Vector3 onPosition;
    public Vector3 offPosition;
    public GameObject lightSwitchObject;
    public float speed = 4.0f;
    private void Update()
    {
        On();
    }
    
    public void On()
    {
        lightSwitchObject.transform.localRotation = Quaternion.Lerp(lightSwitchObject.transform.localRotation, Quaternion.Euler(lightOn ? onPosition : offPosition), Time.deltaTime * speed);
    }

    public void Interact()
    {
        lightOn = !lightOn;
        if (lightOn)
            AudioManagerScript.Instance.PlaySound3D(lightSoundOn, transform.position);
        else
            AudioManagerScript.Instance.PlaySound3D(lightSoundOff, transform.position);
    }
}