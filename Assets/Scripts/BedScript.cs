using UnityEngine;
using UnityEngine.SceneManagement;

public class BedScript : MonoBehaviour, IInteractable
{
    public bool sleep;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GoToSleep();
    }

    public void GoToSleep()
    {
        if (sleep)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void Interact()
    {
        sleep = !sleep;
    }
}
