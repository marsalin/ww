using UnityEngine;

public class LampScript : MonoBehaviour
{
    public Light lamp;
    public float timer;
    public float delay = 2.5f;
    public float flickerSpeed = 1.0f;
    public Vector2 flickerRange = new Vector2(5.0f, 7.0f);
    public Lightswitch lightswitch;

    void Start()
    {
        if (lightswitch == null)
            lightswitch = GameObject.Find("LightSwitch").GetComponent<Lightswitch>();
    }
    // Update is called once per frame
    void Update()
    {
        LightSwitch();
    }

    public void LightSwitch()
    {
        if (!lightswitch.lightOn)
            lamp.intensity = 0.0f;
        else
            LightFlicker();
    }
    public void LightFlicker()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            float flickerTime = Time.time * flickerSpeed;
            lamp.intensity = Map(Mathf.PerlinNoise(flickerTime, 0), 0.0f,1.0f, flickerRange.x, flickerRange.y);
            timer = 0;
        }
    }
    
    float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return newMin + (value-oldMin)*(newMax-newMin)/(oldMax-oldMin);
    }
}