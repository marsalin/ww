using System;
using UnityEngine;

public class CoffeeScript : MonoBehaviour, IInteractable
{
    public Player player;
    public AudioClip drinkSound;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void Interact()
    {
        player.drinkCoffee = !player.drinkCoffee;
        player.coffeeDrinkTimer = 2.0f;
        player.coffeeMoveTimer = 0.4f;
        GetComponent<Collider>().enabled = false;
        AudioManagerScript.Instance.PlaySound2D(drinkSound);
        player.StartCoroutine(player.BlockMovementSeconds(1.5f));
    }
}
