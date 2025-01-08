using UnityEngine;

public class LampScript : MonoBehaviour
{
    public Light lamp;
    public float timer;
    public float delay = 2.5f;
    public float flickerSpeed = 1.0f;
    public Vector2 flickerRange = new Vector2(5.0f, 7.0f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            LightFlicker();
            timer = 0;
        }
    }

    public void LightFlicker()
    {
        float flickerTime = Time.time * flickerSpeed;
        lamp.intensity = Map(Mathf.PerlinNoise(flickerTime, 0), 0.0f,1.0f, flickerRange.x, flickerRange.y);
    }
    
    float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (value-oldMin)*(newMax-newMin)/(oldMax-oldMin);
    }
}