using UnityEngine;

public class DollScript : MonoBehaviour
{
    public AudioClip dollSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DollSound()
    {
        AudioManagerScript.Instance.PlaySound3D(dollSound, transform.position);
    }
}
