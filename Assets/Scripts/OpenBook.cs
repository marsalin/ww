using UnityEngine;

public class OpenBook : MonoBehaviour, IInteractable
{
    public GameObject leftCube;
    public bool bookIsOpen;
    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        OpenTheBook();
    }
    
    public void OpenTheBook()
    {
        leftCube.transform.localRotation = Quaternion.Lerp(leftCube.transform.localRotation, Quaternion.Euler(bookIsOpen ? 90 : -90, 0, 0), Time.deltaTime * 5);
    }
    
    public void Interact()
    {
       bookIsOpen = !bookIsOpen; 
    }
}

