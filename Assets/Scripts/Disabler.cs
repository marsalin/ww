using UnityEngine;

public class Disabler : MonoBehaviour
{
    public RuntimePlatform platformToDisableOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Application.platform == platformToDisableOn)
        {
            gameObject.SetActive(false);
        }
    }
}
