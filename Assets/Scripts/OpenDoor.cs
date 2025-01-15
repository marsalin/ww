using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour, IInteractable
{
    public float speed;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public bool isOpen;
    public AudioClip openSound;
    public AudioClip closeSound;
    public bool openDoor;
    public Collider leftDoorCollider;
    public Collider rightDoorCollider;
    public GameObject wardrobeObject;
    public Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftDoorCollider = leftDoor.GetComponent<Collider>();
        rightDoorCollider = rightDoor.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
       DoorRotation();
    }

    public void DoorRotation()
    {
        leftDoor.transform.localRotation = Quaternion.Lerp(leftDoor.transform.localRotation, Quaternion.Euler(0, isOpen ? 120 : 0, 0), speed * Time.deltaTime);
        rightDoor.transform.localRotation = Quaternion.Lerp(rightDoor.transform.localRotation, Quaternion.Euler(0, isOpen ? -120 : 0, 0), speed * Time.deltaTime);
    }

    public void DoorSound()
    {
        if (!isOpen && openDoor)
        {
            AudioManagerScript.Instance.PlaySound3D(openSound, transform.position);
            openDoor = false;
            leftDoorCollider.enabled = false;
            rightDoorCollider.enabled = false;
        }
        else if (isOpen && !openDoor)
        {
            AudioManagerScript.Instance.PlaySound3D(closeSound, transform.position);
            openDoor = true;
            leftDoorCollider.enabled = true;
            rightDoorCollider.enabled = true;
        }
        else if (!isOpen)
        {
            leftDoorCollider.enabled = true;
            rightDoorCollider.enabled = true;
        }
    }
    
    public void Interact()
    {
        DoorSound();
        isOpen = !isOpen;
        if (wardrobeObject.tag == "WardrobeDoll")
        {
            if (isOpen)
                animator.SetBool("Doll", true);
            else
                animator.SetBool("Doll", false);
        }
    }
}