using UnityEngine;
using UnityEngine.Serialization;

public class CameraScript : MonoBehaviour
{
    public Transform cameraPos;
    public Player player;
    private Vector3 cameraTarget = Vector3.zero;
    public AudioClip endSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.finished)
            transform.position = cameraPos.position;
        else
        {
            if (cameraTarget == Vector3.zero)
                cameraTarget = transform.position + Vector3.forward * 15.0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 2f);
            Vector3 target = Vector3.Lerp(transform.position, cameraTarget, Time.deltaTime * 2f);
            if ((target - transform.position).magnitude > 0.5f * Time.deltaTime) 
                target = transform.position + (target - transform.position).normalized * (0.5f * Time.deltaTime);
            transform.position = target;
        }
    }
}
