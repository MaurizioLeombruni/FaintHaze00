using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private GameManagement manager;
    private Animator animator;
    public string nextScene;

    private LevelUnlock unlocking;

    private void Awake()
    {
        unlocking = GetComponent<LevelUnlock>();
        manager = GameObject.FindGameObjectWithTag("manager").GetComponent<GameManagement>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Controlla se ciò con cui sta collidendo è il giocatore. Se viene premuto il tasto dell'apertura, il gioco controlla se il giocatore
        //ha trovato tutte le chiavi disponibili. La porta si apre se e solo se il giocatore ha trovato tutte le chiavi.

        if (collision.tag == "Player")
        {

            if (Input.GetButtonDown("Open"));
            {
                if (manager.keysFound >= manager.keysNeeded)
                {
                    Debug.Log(collision.GetComponent<Haze>());
                    animator.SetTrigger("Door");
                    Destroy(collision.GetComponent<Haze>());
                    unlocking.UnlockLevel();
                    ChangeScene();
                }
            }

        }
    }

    //Cambia la scena con il nome specificato nella variabile nextScene.
    public void ChangeScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}

