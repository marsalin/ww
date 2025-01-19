using System;
using UnityEngine;

public class Frame : MonoBehaviour, IInteractable
{
    public bool frameMove;
    public float speed = 5.0f;

    private void Update()
    {
        FrameMover();
    }

    public void FrameMover()
    {
        if (frameMove)
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, -13.5f), speed * Time.deltaTime);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, -12.8f), speed * Time.deltaTime);
       
    }
    public void Interact()
    {
        frameMove = !frameMove;
    }
}
