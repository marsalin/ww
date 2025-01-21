using UnityEngine;
using UnityEngine.Serialization;

public class CameraScript : MonoBehaviour
{
    public Transform cameraPos;
    public Player player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.endGame)
            transform.position = cameraPos.position;
        else
        {
            transform.rotation = Quaternion.Lerp(cameraPos.rotation, Quaternion.identity, Time.deltaTime * 10f);
            transform.position += Vector3.forward * Time.deltaTime;
        }
    }
}
