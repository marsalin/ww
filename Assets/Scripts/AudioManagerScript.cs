using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AudioManagerScript : MonoBehaviour
{
    public GameObject jumpscarePrefab;
    public bool hearingSomething;
    public static AudioManagerScript Instance;
    private static bool hasJumpscare;
    void Awake()
    {
        Instance = this;
    }

    public void FootstepSound(AudioClip footstepClip, AudioClip footstepRugClip, GameObject dudeObject)
    {
        float sphereCastRadius = 0.8f;
        float sphereCastRange = 3.0f;
        Vector3 sphereCastCenter = new Vector3(dudeObject.transform.position.x, dudeObject.transform.position.y + 1.0f, dudeObject.transform.position.z);
        RaycastHit hit;
        if (Physics.SphereCast(sphereCastCenter, sphereCastRadius, -dudeObject.transform.up, out hit, sphereCastRange))
        {
            PlaySound3D(hit.collider.CompareTag("Rug") ? footstepRugClip : footstepClip, hit.point);
        }
    }

    public AudioSource PlaySound2D(AudioClip audioClip, float volume = 0.3f, float pitch = 1.0f, bool loop = false)
    {
        if (audioClip == null) return null;
        GameObject clickSoundObject = new GameObject($"Sound_{audioClip.name}");
        AudioSource source = clickSoundObject.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        if (!loop)
            Destroy(clickSoundObject, source.clip.length / pitch);
        source.Play();
        return source;
    }

    public AudioSource PlaySound3D(AudioClip audioClip, Vector3 position, float volume = 0.3f, float pitch = 1.0f, bool loop = false)
    {
        AudioSource source = PlaySound2D(audioClip, volume, pitch, loop);
        if (source == null) return null;
        source.spatialBlend = 1.0f;
        source.gameObject.transform.position = position;
        return source;
    }

    public AudioSource PlaySoundAttached(AudioClip audioClip, Vector3 position, Transform parent, float volume = 1.0f, float pitch = 1.0f, bool loop = false)
    {
        AudioSource audioSource = PlaySound3D(audioClip, position, volume, pitch, loop);
        if (audioSource == null) return null;
        audioSource.gameObject.transform.parent = parent;
        return audioSource;
    }

    public IEnumerator HearingSomething()
    {
        hearingSomething = true;
        yield return new WaitForSeconds(8.0f);
        hearingSomething = false;
    }
    private IEnumerator SpawnJumpScare()
    {
        Instantiate(jumpscarePrefab);
        yield return new WaitForSeconds(1.0f);
        hasJumpscare = true;
        Application.Quit();
    }
    static bool canQuit()
    {
        if (hasJumpscare)
            return true;
        Instance.StartCoroutine(Instance.SpawnJumpScare());
        return hasJumpscare;
    }
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += canQuit;
    }
}