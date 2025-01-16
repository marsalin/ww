using System;
using UnityEngine;

public class Cat : MonoBehaviour, IInteractable
{
    public Animator animator;
    public AudioClip catSound;

    public void AnimationEnd(AnimationEvent animationEvent)
    {
        animator.ResetTrigger("Pet");
    }
    public void Interact()
    {
        animator.SetTrigger("Pet");
        AudioManagerScript.Instance.PlaySound3D(catSound, transform.position);
    }
}
