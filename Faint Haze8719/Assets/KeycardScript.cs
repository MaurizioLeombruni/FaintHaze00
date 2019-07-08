using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardScript : MonoBehaviour
{
    private GameManagement manager;

    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager").GetComponent<GameManagement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
            SoundManagerScript.PlaySound("CartaMagnetica");
        manager.keysFound = manager.keysFound + 1;
        Debug.Log(collision);
        Debug.Log("key added");
        Destroy(gameObject);
    }
}
