using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Haze : PhysicsObject
{
    //Parametri di base del personaggio.
    public float speed;
    public float jump_force;

    //Valori di stato del personaggio, usati per la detezione da parte dei nemici.
    //Lo stato Visible è quando il personaggio è pienamente visibile; Caution è per quando è solo parzialmente visibile (intangibile dentro
    //un oggetto che non lo copre totalmente); Hidden è per quando il personaggio è completamente nascosto.
    public enum Visibility { Visible, Caution, Hidden }
    public Visibility stealth_status = Visibility.Visible;
    //La direzione base e del rampino, utilizzate per monitorare sia il movimento che la direzione dello sprite.
    public enum Direction { Left, Right }
    public Direction direct = Direction.Right;
    public Direction hs_direct = Direction.Right;

    //Parametri di supporto per le funzionalità base del personaggio, come il movimento.
    protected Vector2 move_save;
    protected float hookshot_position;

    //Parametri di controllo per l'aspetto grafico del personaggio.
    public SpriteRenderer sprite;
    //animator goes here

    //Parametri di controllo per vedere se il personaggio può compiere l'azione specificata.
    public bool canUseSkill01;
    public bool canUseSkill02;
    public bool canClimb;
    public bool canMove;
    public bool canUseHookshot;

    //Parametri di controllo per vedere le condizioni attuali del personaggio.
    public bool isStuck = false;
    public bool isCrouched;
    public bool isClimbing;
    public bool isIntangible;
    private bool flipSprite;

    //Parametri della Skill 1 (Intangibilità).
    public int skill01_duration;
    public int skill01_cooldown;
    public int skill01_stunduration;

    //Parametri della Skill 2 (Lancio feromoni).
    public GameObject skill02_object;
    public GameObject skill02_object_instance;
    public Transform skill02_throw_point;
    public int skill02_cooldown;

    //Valori di funzionalità del rampino.
    private float rc_distance;
    private float rc_target_distance;
    public LayerMask rc_hookshot_mask = 10;
    private RaycastHit2D rc_hookshot_hit;

    //Parametri di controllo della User Interface.
    public Text hookshot_indicator;

    IEnumerator Skill01Cooldown()
    {
        yield return new WaitForSeconds(skill01_cooldown);

        canUseSkill01 = true;
    }

    IEnumerator Stun()
    {
        Debug.Log("Stunned");

        speed = 0;

        yield return new WaitForSeconds(skill01_stunduration);

        speed = 5;
    }

    IEnumerator Skill01()
    {
        canUseSkill01 = false;
        isIntangible = true;

        //anim.SetTrigger("Intangible");

        gameObject.layer = 9;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

        yield return new WaitForSeconds(skill01_duration);

        gameObject.layer = 0;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

        if (isStuck)
        {
            StartCoroutine("Stun");
        }

        isIntangible = false;
        StartCoroutine("Skill01Cooldown");
    }

    IEnumerator Skill02()
    {
        canUseSkill02 = false;

        skill02_object_instance = Instantiate(skill02_object, skill02_throw_point.position, skill02_throw_point.rotation);

        yield return new WaitForSeconds(skill02_cooldown);

        canUseSkill02 = true;
        
    }

    IEnumerator JumpDirection()
    {
        yield return new WaitForSeconds(0.15f);

        canMove = false;

        Debug.Log("NON MI POSSO MUOVERE");
        yield return new WaitUntil(GroundedCheck);

        Debug.Log("MI POSSO MUOVERE");
        canMove = true;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    IEnumerator ActiveHookshot()
    {
        move_save = Vector2.zero;
        if(hs_direct==Direction.Right && direct==Direction.Left)
        {
            direct = Direction.Right;
            sprite.flipX = !sprite.flipX;
        }
        if(hs_direct==Direction.Left && direct == Direction.Right)
        {
            direct = Direction.Left;
            sprite.flipX = !sprite.flipX;
        }

        canMove = false;

        rb2d.velocity = new Vector2(0, 15);

        //Debug.Log(rb2d.velocity);

        yield return new WaitUntil(HeightCheck);

        if (hs_direct == Direction.Right)
            rb2d.velocity = new Vector2(10, 0);
        else
            rb2d.velocity = new Vector2(-10, 0);

        yield return new WaitForSeconds(0.1f);

        rb2d.velocity = Vector2.zero;
        //Debug.Log("gachiBASS");
        canMove = true;
    }

    public void Death()
    {
        //anim.SetTrigger("Death");
        Time.timeScale = 0;
        //gameOverPanel.SetActive(true);
    }

    public void OnDrawGizmos()
    {
        //Robe di debug
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 10, 0));
    }


    public bool GroundedCheck()
    {
        if (grounded)
            return true;
        else
            return false;
    }

    /*public bool HeightCompare(Vector3 first, Vector3 second)
    {
        if (first.y >= second.y + 5f)
        {
            Debug.Log(first.y);
            Debug.Log(second.y);
            return true;
        }

        else
            return false;
    }*/

    public bool HeightCheck()
    {
        if (transform.position.y > hookshot_position)
            return true;
        else
            return false;
    }

    private void Start()
    {
        speed = 5;
        jump_force = 8;

        canUseSkill01 = true;
        canUseSkill02 = true;
        canMove = true;

        rc_distance = 10f;
        Physics2D.IgnoreLayerCollision(11, 13);
    }

    protected override void ComputeVelocity()                                                  
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (canMove)
        {
            targetVelocity = move * speed;
            move_save = move;

            flipSprite = (sprite.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
            
            if (flipSprite)
            {
                sprite.flipX = !sprite.flipX;
                if (direct == Direction.Right)
                    direct = Direction.Left;
                else
                    direct = Direction.Right;
            }

        }
        else
            targetVelocity = move_save * speed;

        if (Input.GetButtonDown("Jump") && grounded)
        {

            rb2d.bodyType = RigidbodyType2D.Dynamic;
            if (rb2d.velocity != Vector2.zero)
            {
                Debug.Log("fghjk" + move);
                rb2d.AddForce(move * jump_force, ForceMode2D.Impulse);
            }
            else
            {
                rb2d.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);
                Debug.Log(targetVelocity);
            }

            StartCoroutine("JumpDirection");
        }


       

        if (Input.GetKeyDown(KeyCode.B) && grounded)
        {
            if (Physics2D.Raycast(transform.position, Vector2.up, rc_distance, rc_hookshot_mask.value))
            {

                hookshot_position = Physics2D.Raycast(transform.position, Vector2.up, rc_distance, rc_hookshot_mask.value).transform.position.y;
                rc_hookshot_hit = Physics2D.Raycast(transform.position, Vector2.up, rc_distance, rc_hookshot_mask.value);

                if (rc_hookshot_hit.collider.gameObject.tag == "hookshot_point_right")
                {
                    hs_direct = Direction.Right;
                    StartCoroutine("ActiveHookshot");
                }

                else if (rc_hookshot_hit.collider.gameObject.tag == "hookshot_point_left")
                {
                    hs_direct = Direction.Left;
                    StartCoroutine("ActiveHookshot");
                }

                else if (rc_hookshot_hit.collider.gameObject.tag == "hookshot_interaction")
                    rc_hookshot_hit.collider.gameObject.GetComponent<InteractionWasps>().ReduceEpsilonDetection();

            }

        }

        


       

    }

    private void LateUpdate()
    {
        if (Input.GetButtonDown("Intangibility"))
        {
            if (canUseSkill01)
            {
                StartCoroutine("Skill01");
            }
        }

        if (Input.GetButtonDown("Throw"))
        {
            if (canUseSkill02)
            {
                StartCoroutine("Skill02");
            }
        }
    }

}
