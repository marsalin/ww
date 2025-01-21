using UnityEngine;
using UnityEngine.Serialization;

public class OpenBook : MonoBehaviour, IInteractable
{
    [FormerlySerializedAs("leftCube")] public GameObject openable;
    [FormerlySerializedAs("bookIsOpen")] public bool isOpen;
    public Vector3 openPosition;
    public Vector3 closePosition;
    public AudioClip openSound, closeSound;
    public float speed = 4.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Open();
        
    }
    
    public void Open()
    {
        openable.transform.localRotation = Quaternion.Lerp(openable.transform.localRotation, Quaternion.Euler(isOpen ? openPosition : closePosition), Time.deltaTime * speed);
    }
    public void Sound()
    {
        if (!isOpen)
            AudioManagerScript.Instance.PlaySound3D(openSound, transform.position);
        else
            AudioManagerScript.Instance.PlaySound3D(closeSound, transform.position);
    }
    public void Interact()
    {
       isOpen = !isOpen;
       Sound();
    }
}

