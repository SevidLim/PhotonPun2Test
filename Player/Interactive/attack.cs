using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    public PlayerMovement playermovement;

    public float damage;

    void Start()
    {
        
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playermovement = other.GetComponent<PlayerMovement>();
            playermovement.hp -= damage;

            Debug.Log(other + "Get Attack" + playermovement.hp);
        }
    }
}
