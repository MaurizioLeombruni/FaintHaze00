﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : PhysicsObject
{
    private Haze player;

    public Collider2D limiteDestro;
    public Collider2D limiteSinistro;

    public bool facingRight = true;                             
    public bool isFlipping = false;
    public bool playerSpottedMiddle = false;
    public bool playerSpottedUp = false;
    public bool playerSpottedDown = false;

    public float speed;                             //La velocità base del nemico
    public float speedWithPheromones;               //La variabile per calibrare la velocità con cui raggiungerà i feromoni e il punto di ritorno usando il transform.Position

    public Transform startRayCastMiddle;
    public Transform endRayCastMiddle;

    public Transform startRayCastUp;
    public Transform endRayCastUp;

    public Transform startRayCastDown;
    public Transform endRayCastDown;

    public enum Status {Ronda, Attratto, Stordito, Ritorno};
    public Status ActiveStatus;
    public Vector2 pheromonePosition;

    public Transform returnPointPosition;

    private Transform enemyTransform;

    private Quaternion rightRotation = Quaternion.identity;
    private Quaternion leftRotation = Quaternion.Euler(0, 180, 0);





    //int m;
    

    void Start()
    {
        player = FindObjectOfType<Haze>();
        ActiveStatus = Status.Ronda;                                                            //Status iniziale del nemico. Comincia sempre in ronda

        Debug.Log(gameObject.layer);
    }


    protected override void ComputeVelocity()
    {
        switch (ActiveStatus)
        {
            case Status.Ronda:                         //Status di movimento base, Comincia in questo status


                limiteDestro.enabled = true;
                limiteSinistro.enabled = true;


                if (facingRight == true)
                {
                    rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
                   // rb2d.MovePosition(Vector3.right * speed);
                }
                else
                {
                    //rb2d.MovePosition(Vector3.left * -speed);
                    rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
                }
                


                //Debug LineCast
                
                
                RaycastHit2D hit1 = Physics2D.Linecast(startRayCastMiddle.position, endRayCastMiddle.position);
                Debug.DrawLine(startRayCastMiddle.position, endRayCastMiddle.position, Color.green);

                RaycastHit2D hit2 = Physics2D.Linecast(startRayCastUp.position, endRayCastUp.position);
                Debug.DrawLine(startRayCastUp.position, endRayCastUp.position, Color.red);

                RaycastHit2D hit3 = Physics2D.Linecast(startRayCastDown.position, endRayCastDown.position);
                Debug.DrawLine(startRayCastDown.position, endRayCastDown.position, Color.blue);


                //LineCast: Rapresenta il cono di visione

                playerSpottedMiddle = Physics2D.Linecast(startRayCastMiddle.position, endRayCastMiddle.position, 1 << LayerMask.NameToLayer("Player"));
                playerSpottedUp = Physics2D.Linecast(startRayCastUp.position, endRayCastUp.position, 1 << LayerMask.NameToLayer("Player"));
                playerSpottedDown = Physics2D.Linecast(startRayCastDown.position, endRayCastDown.position, 1 << LayerMask.NameToLayer("Player"));


                if ((playerSpottedDown == true) || (playerSpottedMiddle == true) || (playerSpottedUp == true))
                {
                    //Debug.Log("Start Kill Player Animation");
                    //ammazza il player

                }
                break;


            case Status.Attratto:                                               //Stauts che comincia quando il nemico entra nel Collider Esterno dei feromoni(In questo Stato il raycast per il cono di visione non è presente)

                CheckRotation(pheromonePosition.x);

                limiteDestro.enabled = false;
                limiteSinistro.enabled = false;
                //Physics2D.IgnoreLayerCollision(10, 12,true);                   //Disattiva la collisione con i limiti della sua ronda


                transform.position = Vector2.MoveTowards(transform.position, new Vector3( pheromonePosition.x, transform.position.y, transform.position.z),speedWithPheromones*Time.deltaTime);     //Il tempo che ci mette a raggiungere il punto centrale dei feromoni.     


                break;
            case Status.Stordito:                                           //Status che si attiva quando raggiunge il Collider Interno dei feromoni

                

                rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

                


                break;
            case Status.Ritorno:

                CheckRotation(returnPointPosition.position.x);

                //RitornoCoroutine();
                rb2d.constraints = RigidbodyConstraints2D.None;

                transform.position = Vector2.MoveTowards(transform.position, returnPointPosition.position, 2 * Time.deltaTime);         //Ritorna verso il ReturnPoint (Un punto idealmente al centra della ronda del singolo nemico)

                
                //Riparte il cono di visione

                Debug.DrawLine(startRayCastMiddle.position, endRayCastMiddle.position, Color.green);


                Debug.DrawLine(startRayCastUp.position, endRayCastUp.position, Color.red);

               
                Debug.DrawLine(startRayCastDown.position, endRayCastDown.position, Color.blue);


                //LineCast

                playerSpottedMiddle = Physics2D.Linecast(startRayCastMiddle.position, endRayCastMiddle.position, 1 << LayerMask.NameToLayer("Player"));
                playerSpottedUp = Physics2D.Linecast(startRayCastUp.position, endRayCastUp.position, 1 << LayerMask.NameToLayer("Player"));
                playerSpottedDown = Physics2D.Linecast(startRayCastDown.position, endRayCastDown.position, 1 << LayerMask.NameToLayer("Player"));


                Physics2D.IgnoreLayerCollision(10, 12, false);

               

                break;

        }
        





    }

   

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger debug");

        if (collision.gameObject.tag=="LimitLeft"&& collision==limiteSinistro)                                      // Quando collide con uno dei Emptygameobject "Limite" cambia direzione
        {
          
            

            StartCoroutine(CoroutineLeft());
            
            
        }
        if (collision.gameObject.tag == "LimitRight"&& collision==limiteDestro)
        {

            StartCoroutine(CoroutineRight());

        }

        if (collision.gameObject.tag=="ReturnPoint" && ActiveStatus == Status.Ritorno )
        {
            ActiveStatus = Status.Ronda;
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision debug");
    }



    IEnumerator CoroutineLeft()                                                     // Quando collidono con i limiti rimangono fermi per Tot secondi (3), poi si girano e proseguono nella direzione opposta
    {


        rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;


        yield return new WaitForSeconds(3);
        rb2d.constraints = RigidbodyConstraints2D.None;

        facingRight = true;
        transform.rotation = rightRotation;
        
    }

    IEnumerator CoroutineRight()
    {

        rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        yield return new WaitForSeconds(3);
        rb2d.constraints = RigidbodyConstraints2D.None;

        facingRight = false;
        transform.rotation = leftRotation;
       
        
    }






    private void CheckRotation(float xPos)                          //Controlla in che direzione è girato il nemico controllando la il transform X
    {
        if(xPos > transform.position.x)
        {
            transform.rotation = rightRotation;
            facingRight = true;
        }
        else
        {
            transform.rotation = leftRotation;
            facingRight = false;
        }

    }


}



