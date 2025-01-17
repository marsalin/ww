using System.Collections;
using UnityEngine;

public class JumpscareScript : MonoBehaviour
{
    public AudioClip jumpscareSound;
    public float timer = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Jumpscare());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Jumpscare()
    {
        AudioManagerScript.Instance.PlaySound2D(jumpscareSound);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        } 
        Destroy(gameObject);
    }
}
