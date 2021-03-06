﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneBottleBaseScript : MonoBehaviour
{
    public GameObject innerCollider;
    public GameObject outerCollider;
    public GameObject smokeBase;
    private Rigidbody2D rb2d;
    private Haze player;
    public float speed;
    public Animator bottleAnimation;

    private EnemyMovement enemyScript;
    
   

    public bool activePheromones=false;


    //Controlla la direzione in cui è il player e decide con quale forza lanciare il feromone

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bottleAnimation= GetComponent<Animator>();
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Haze>();

        SoundManagerScript.PlaySound("FeromoniLancio");

        if (player.direct == Haze.Direction.Right)
        {
            rb2d.AddForce(Vector2.right * speed);
        }

        else if (player.direct == Haze.Direction.Left)
        {

            rb2d.AddForce(Vector2.left * speed);
        }



    }



    //Rimane Attivo per tot secondi, dopo cui viene distrutto. Quando viene distrutto, cambia lo status dei nemici con lo status "Stordito" in "Ritorno".

    IEnumerator PheromonesActiveColliders()
    {
        innerCollider.SetActive(true);
        outerCollider.SetActive(true);
        

       
        smokeBase.SetActive(true);
        yield return new WaitForSeconds(10);
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            
        
            enemyScript = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyMovement>();


            if (enemyScript.ActiveStatus == EnemyMovement.Status.Stordito)
            {
                enemyScript.ActiveStatus = EnemyMovement.Status.Ritorno;
            }
        }
        Debug.Log("ok");
        Destroy(gameObject);
    }


    //Una volta che collide con il pavimento viene resa immobile

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "floor")
        {


            activePheromones = true;
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            bottleAnimation.SetTrigger("touchedTheGround");

            StartCoroutine("PheromonesActiveColliders");

        }
    }


}
