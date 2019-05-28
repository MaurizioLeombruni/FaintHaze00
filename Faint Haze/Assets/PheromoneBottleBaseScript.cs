using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneBottleBaseScript : MonoBehaviour
{
    public GameObject innerCollider;
    public GameObject outerCollider;
    private Rigidbody2D rb2d;
    private Haze player;
    public float speed;

    private EnemyMovement enemyScript;
    
   

    public bool activePheromones=false;


    //Controlla la direzione in cui è il player e decide con quale forza lanciare il feromone

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        player=GameObject.FindGameObjectWithTag("Player").GetComponent<Haze>();



        if (player.direct == Haze.Direction.Right)
        {
            rb2d.AddForce(Vector2.right * speed);
        }

        else if (player.direct == Haze.Direction.Left)
        {

            rb2d.AddForce(Vector2.left * speed);
        }



    }


    //Una volta che collide con il pavimento viene resa immobile

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "floor")
        {
            
            
            activePheromones = true;
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY ;

            StartCoroutine("PheromonesActiveColliders");

            
           

        }
    }

    //Rimane Attivo per tot secondi, dopo cui viene distrutto. Quando viene distrutto, cambia lo status dei nemici con lo status "Stordito" in "Ritorno".

    IEnumerator PheromonesActiveColliders()
    {
        innerCollider.SetActive(true);
        outerCollider.SetActive(true);
        yield return new WaitForSeconds(10);
        enemyScript = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyMovement>();


        /*if (EnemyScript.ActiveStatus == EnemyMovement.Status.Stordito)
        {
            EnemyScript.ActiveStatus = EnemyMovement.Status.Ritorno;
        }*/


        Destroy(gameObject);
    }







}
