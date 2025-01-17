using UnityEngine;

public class Lightswitch : MonoBehaviour, IInteractable
{
    public bool lightOn;
 
    public void Interact()
    {
        lightOn = !lightOn;
    }
}
