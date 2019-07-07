using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Haze : MonoBehaviour
{
    //Parametri di base del personaggio.
    public float speed;
    private float speed_aux;
    public float jump_force;
    private Rigidbody2D rb2d;
    public ContactFilter2D contactFilter;
    private Vector2 velocity;

    private GameManagement manager;

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
    public Transform groundCheckPos;
    public float circleRadius;

    //Parametri di controllo per l'aspetto grafico del personaggio.
    public SpriteRenderer sprite;
    public Sprite sprite_up;
    public Sprite sprite_down;

    public BoxCollider2D collider_up;
    public CircleCollider2D collider_down;

    public GameObject filtro;

    public Animator haze_anim;

    private float velocity_ctrl;

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
    public bool isStunned;
    public bool isGrounded;
    public bool isDead;

    private bool skill01_timesup;

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
    public float hookshot_speed;
    public LayerMask rc_hookshot_mask = 10;
    private RaycastHit2D rc_hookshot_hit;

    //Parametri di controllo della User Interface.
    public GameObject gameOverPanel;

    IEnumerator Skill01CheckTimer()
    {
        //Dopo tot secondi, il giocatore viene forzatamente costretto ad interrompere l'intangibilità.

        yield return new WaitForSeconds(skill01_duration);

        if(Input.GetButton("Intangibility"))
            skill01_timesup = true;
    }

    IEnumerator Skill01Cooldown()
    {
        //Aspetta i secondi specificati nella variabile intera "skill01_cooldown" per riabilitare l'uso dell'intangibilità.

        yield return new WaitForSeconds(skill01_cooldown);

        canUseSkill01 = true;
        skill01_timesup = false;
        haze_anim.SetBool("tired_anim", false);
    }

    IEnumerator Stun()
    {
        //Stordisce il personaggio per la durata specificata nella variabile "skill01_stunduration".
        //Il movimento e la capacità di compiere azioni e usare oggetti o abilità vengono disabilitati tramite le due variabili booleane "canMove" e "isStunned".
        //canMove disabilita il movimento. isStunned previene che il giocatore compia azioni come rampino o feromoni durante lo stordimento.

        canMove = false;
        speed = 0;
        isStunned = true;

        //Costringe il personaggio a rialzarsi se è accucciato.
        if(isCrouched)
            GetUpFromCrouch();

        //Gestisce le animazioni. L'animazione di crouching viene resettata a prescindere, in modo tale da essere sempre disponibile per essere riutilizzata
        //anche dopo lo stordimento.
        haze_anim.SetBool("stunned_anim", true);
        haze_anim.SetBool("isCrouching_anim", false);


        yield return new WaitForSeconds(skill01_stunduration);

        //Dopo aver esaurito il tempo di stordimento, le capacità del personaggio vengono ripristinate.

        canMove = true;
        speed = speed_aux;
        isStunned = false;
        haze_anim.SetBool("stunned_anim", false);
    }

    IEnumerator Skill01()
    {
        //Questa coroutine gestisce l'uso dell'abilità dell'Intangibilità.
        //Per prima cosa disabilita l'uso ripetuto dell'abilità tramite la booleana "canUseSkill01". Poi setta la booleana isIntangible per
        //comunicare al gioco che il personaggio è adesso intangibile. Ciò serve agli oggetti per far partire i rispettivi script.

        canUseSkill01 = false;
        isIntangible = true;

        //Richiama la funzione di controllo dell'intangiblità, passandogli il valore booleano "falso" - per far capire al gioco che deve attivare
        //gli effetti della skill.
        Skill01Control(false);

        yield return new WaitUntil(Skill01Timer);

        //Richiama la funzione di controllo dell'intangiblità, passandogli il valore booleano "vero" - per far capire al gioco che deve far terminare
        //gli effetti della skill.
        Skill01Control(true);
        haze_anim.SetBool("tired_anim", true);
    }

    IEnumerator Skill02()
    {
        //Questa coroutine gestisce l'uso dell'abilità dei Feromoni.
        //Per prima cosa disabilita l'uso ripetuto dell'abilità tramite la booleana "canUseSkill02". Poi instanzia il Game Object dei feromoni, prendendo come
        //coordinate la posizione del Game Object vuoto legato al personaggio di Haze.
        canUseSkill02 = false;
        haze_anim.SetTrigger("isTrowing_anim");
        skill02_object_instance = Instantiate(skill02_object, skill02_throw_point.position, skill02_throw_point.rotation);

        //Attende la durata specificata nella variabile skill02_cooldown per far terminare lo script.

        yield return new WaitForSeconds(skill02_cooldown);

        //Riabilita l'uso dell'abilità.
        
        canUseSkill02 = true;
        
    }

    IEnumerator JumpDirection()
    {
        //Questa coroutine gestisce la funzione del salto.
        //La coroutine aspetta un decimo di secondo per far partire la funzione del salto. Ciò evita eventuali bug o contrasti con
        //l'abilità del movimento.
        yield return new WaitForSeconds(0.15f);

        //Disabilita il normale movimento del personaggio.
        canMove = false;

        //Attende che il personaggio torni a terra per riabilitare il movimento.
        yield return new WaitUntil(GroundedCheck);

        //Riabilita il movimento del personaggio e gli restituisce la fisica personalizzata.
        canMove = true;
        //rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    IEnumerator ActiveHookshot()
    {
        //Questa coroutine gestisce la funzione del rampino.
        //Per prima cosa azzera completamente la velocità del personaggio, per evitare eventuali bug dovuti alla fisica personalizzata e al movimento.
        isGrounded = false;
        move_save = Vector2.zero;

        //Verifica se c'è un contrasto tra la direzione del personaggio e quella del punto d'appiglio. Se le due non coincidono, il personaggio viene
        //fatto ruotare nella stessa direzione del punto d'appiglio. Ciò crea un effetto visivo più coerente.
        if (hs_direct==Direction.Right && direct==Direction.Left)
        {
            direct = Direction.Right;
            sprite.flipX = !sprite.flipX;
        }
        if(hs_direct==Direction.Left && direct == Direction.Right)
        {
            direct = Direction.Left;
            sprite.flipX = !sprite.flipX;
        }

        //Una volta corretto (se necessario) la direzione del personaggio, si disabilita il movimento del personaggio.
       

        //Al personaggio viene data una velocità puramente verticale.
        rb2d.velocity = new Vector2(rb2d.velocity.x, hookshot_speed);

        //Aspetta fin quando il personaggio non raggiunge l'altezza designata.

        yield return new WaitUntil(HeightCheck);

        //In base alla direzione (corretta in precedenza) del personaggio, esso viene spinto sulla piattaforma.
        if (hs_direct == Direction.Right)
            rb2d.velocity = new Vector2(10, 0);
        else
            rb2d.velocity = new Vector2(-10, 0);

        //Aspetta un decimo di secondo per stabilizzare le funzioni del gioco.
        yield return new WaitForSeconds(0.1f);

        //Azzera nuovamente la velocità per contrastare eventuali bug con la fisica personalizzata. Dopodiché ripristina l'abilità di movimento.
        rb2d.velocity = Vector2.zero;
        canMove = true;
    }

    public void Death()
    {
        canMove = false;
        canUseHookshot = false;
        canUseSkill01 = false;
        canUseSkill02 = false;
        canClimb = false;
        isDead = true;
        haze_anim.SetTrigger("death_anim_t");
        manager.GameOver();
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void OnDrawGizmos()
    {
        //Una funzione puramente di debug che serve a controllare la funzione del rampino.
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 10, 0));

        Gizmos.DrawWireSphere(groundCheckPos.position, circleRadius);
    }

    public bool GroundedCheck()
    {
        //Una funzione puramente di supporto che controlla se il personaggio è a terra o meno.

        if (isGrounded)
            return true;
        else
            return false;
    }

    public void GroundControl()
    {
        //Instanzia un cerchio invisibile poco sotto i piedi del personaggio. Il controllo dello stato a terra dipende dalla collisione di questo
        //cerchio con il terreno.
        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, circleRadius, 1 << LayerMask.NameToLayer("Ground"));


        //Cambia l'animazione in base a se il personaggio sta cadendo o sia a terra.
        if (isGrounded)
            haze_anim.SetBool("falling_anim", false);
        else
            haze_anim.SetBool("falling_anim", true);
    }

    public bool HeightCheck()
    {
        //Una funzione di supporto per il rampino. Controlla l'altezza del personaggio rispetto all'altezza specificata dal punto d'appiglio.
        //Serve ad arrestare la salita del personaggio quando arriva all'altezza giusta.

        if (transform.position.y > hookshot_position)
            return true;
        else
            return false;
    }

    public bool Skill01Timer()
    {
        //Funzione di supporto che controlla se il giocatore esaurisce il tempo di intangibilità.

        if (skill01_timesup)
            return true;
        else
            return false;
    }

    public void Skill01Control(bool check)
    {
        //Funzione di supporto che attiva o disattiva gli effetti dell'intangibilità.

        if (check)
        {
            //Se il giocatore è già intangibile quando la funzione viene chiamata, disattiva gli effetti dell'intangibilità.
            //Ripristina la normale capacità di collisione.
            gameObject.layer = 0;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            manager.filterActive = false;
            //La booleana isStuck viene settata a vero dagli ostacoli quando il personaggio ci entra dentro, e viene messa a falso quando il personaggio esce.
            //Se il personaggio è ancora all'interno dell'ostacolo quando l'abilità termina, fa partire la funzione dello stordimento.
            if (isStuck)
            {
                StartCoroutine("Stun");
            }

            //Disabilita la condizione di intangibilità, facendo tornare il personaggio nello stato normale e facendo partire il cooldown.
            isIntangible = false;
            StartCoroutine("Skill01Cooldown");
        }
        else
        {
            //Se il giocatore non è già intangibile quando la funzione viene chiamata, attiva gli effetti dell'intangibilità.
            //Cambia il layer del personaggio, alterando la sua capacità di collidere con determinati oggetti. Serve principalmente per entrare
            //negli oggetti designati. Dopodiché aspetta per la durata dell'abilità specificata nella variabile skill01_duration per far proseguire
            //lo script e terminare gli effetti dell'intangibilità.
            gameObject.layer = 9;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            manager.filterActive = true;
        }
    }

    public void GetUpFromCrouch()
    {
        //La seguente funzione costringe il personaggio a rialzarsi (e terminare la fase di crouching).
        //Utilizzata principalmente quando il personaggio viene stordito dopo un errore nell'utilizzo dell'intangibilità. Ciò costringe Haze a rialzarsi
        //anche senza input dal giocatore.
        //Non è necessaria una funzione equivalente per l'inizio del crouching in quanto esso avviene solo e unicamente per input del giocatore.

        Debug.Log("Crouch");
        speed = speed * 2;
        collider_up.enabled = true;
        collider_down.enabled = false;
        isCrouched = false;
        sprite.sprite = sprite_up;

        //Se il personaggio non è stordito, l'animazione di rialzarsi procede normalmente. Se il personaggio è stordito, l'animazione viene resettata in seguito
        //per dare precedenza all'animazione di stordimento.
        if(!isStunned)
            haze_anim.SetBool("isCrouching_anim", false);
    }

    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager").GetComponent<GameManagement>();
        haze_anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //La funzione Start inizializza alcune importanti variabili quando il giocatore spawna nel livello.

        speed = 5;
        speed_aux = speed;

        canUseSkill01 = true;
        canUseSkill02 = true;
        canMove = true;

        rc_distance = 10f;
        Physics2D.IgnoreLayerCollision(11, 13);
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()                                                  
    {
        //Controlla, ogni frame, se è il personaggio è a terra o meno.
        GroundControl();

        //Controlla se il personaggio può muoversì. Se può, questa funzione entra in azione e gestisce il movimento standard.
        if (isGrounded & !isDead)
        {
            //Stabilisce la velocità calcolandola secondo la direzione presa in input e la velocità del personaggio.
            //Inoltre salva la direzione in una variabile ausiliaria.
            Vector2 move = Vector2.zero;
            move.x = Input.GetAxisRaw("Horizontal");
            rb2d.velocity = new Vector2(move.x * speed, 0);
            speed = speed_aux;
            move_save = move;
            haze_anim.SetFloat("velocity_anim", Mathf.Abs(rb2d.velocity.x));

            //Gira il personaggio quando cambia direzione.
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
     //Per sopra: la seconda targetVelocity viene usata durante il salto: quando il personaggio non può muoversi ma ha comunque una velocità,
        //continua nel suo moto senza aver bisogno dell'input del giocatore.


        //Il salto. Per saltare controlla che il personaggio sia a terra e non sia stordito. In input prende il pulsante del salto.
        if (Input.GetButtonDown("Jump") && isGrounded && !isStunned && !isDead)
        {
            Debug.Log("Jumping");

            StartCoroutine("JumpDirection");
            //Il salto varia in base al fatto se il personaggio si sia muovendo o meno.
            if (rb2d.velocity != Vector2.zero)
            {
                //Se si sta muovendo, il salto viene calcolato in base alla direzione e la forza del salto.
                haze_anim.SetTrigger("jump_anim");
                rb2d.velocity = ((Vector2.up + move_save) * jump_force * Time.fixedDeltaTime);

            }
            else
            {
                //Altrimenti, il salto viene calcolato in base alla sola forza del salto, con direzione standard in sù.
                haze_anim.SetTrigger("jump_anim");
                rb2d.velocity = (Vector2.up * jump_force * Time.fixedDeltaTime);
            }

            //Fa partire la coroutine che gestisce il movimento durante il salto.
        }


       
        //Il rampino. Per essere usato controlla se il personaggio sia a terra e non sia stordito. Prende in input il pulsante specificato.
        if (Input.GetButtonDown("Hookshot") && isGrounded && !isStunned && !isCrouched)
        {
            //Quando azionato, controlla se effettivamente c'è qualcosa che possa essere usato nella funzione. Il rampino parte solo e unicamente
            //quando c'è un elemento sopra il personaggio che ne permette l'uso.
            if (Physics2D.Raycast(transform.position, Vector2.up, rc_distance, rc_hookshot_mask.value))
            {
                //Prende le informazioni riguardo l'oggetto colpito, specificamente la sua posizione.
                hookshot_position = Physics2D.Raycast(transform.position, Vector2.up, rc_distance, rc_hookshot_mask.value).transform.position.y;
                rc_hookshot_hit = Physics2D.Raycast(transform.position, Vector2.up, rc_distance, rc_hookshot_mask.value);


                //Gli effetti variano a seconda dell'elemento colpito. Se è un punto d'appiglio, controlla la sua direzione e fa partire lo script
                //del rampino. Altrimenti, se è un vespaio (interazione ambientale), fa partire lo script dell'interazione.
                if (rc_hookshot_hit.collider.gameObject.tag == "hookshot_point_right")
                {
                    hs_direct = Direction.Left;
                    StartCoroutine("ActiveHookshot");
                }

                else if (rc_hookshot_hit.collider.gameObject.tag == "hookshot_point_left")
                {
                    hs_direct = Direction.Right;
                    StartCoroutine("ActiveHookshot");
                }

                else if (rc_hookshot_hit.collider.gameObject.tag == "hookshot_interaction")
                    rc_hookshot_hit.collider.gameObject.GetComponent<InteractionWasps>().ReduceEpsilonDetection();

            }

        }

    }

    //La funzione LateUpdate viene chiamata 1 frame dopo ComputeVelocity. Ciò serve ad evitare contrasti con le varie funzioni che possono essere
    //attivate nello stesso frame.
    private void LateUpdate()
    {
        //Se il pulsante dell'intangibilità viene premuto, e se si può effettivamente usare l'abilità, aziona il rispettivo script.
        if (Input.GetButton("Intangibility"))
        {
            if (canUseSkill01)
            {
                StartCoroutine("Skill01");
                StartCoroutine("Skill01CheckTimer");
            }
        }

        //Interrompe tutte le funzioni dell'intangibilità. Poi richiama manualmente la funzione di controllo della skill, per far terminare
        //gli effetti della skill senza dover attendere la fine della coroutine.
        if(Input.GetButtonUp("Intangibility") && isIntangible)
        {
            StopCoroutine("Skill01");
            StopCoroutine("Skill01CheckTimer");

            Skill01Control(true);
        }

        //Stessa cosa per il lancio dei feromoni.
        if (Input.GetButtonDown("Throw"))
        {
            if (canUseSkill02)
            {
                StartCoroutine("Skill02");
            }
        }

        //Il crouching.
        //Se il personaggio può muoversi normalmente, alla pressione del pulsante specificato verifica se il personaggio sia già abbassato.
        //In base allo stato del personaggio, cambia il collider e lo stato di crouching del personaggio.
        if (Input.GetButtonDown("Crouch") && isGrounded == true && canMove == true && isCrouched == false)
        {
            Debug.Log("Crouch");

            speed = speed / 2;
            collider_down.enabled = true;
            collider_up.enabled = false;
            isCrouched = true;
            sprite.sprite = sprite_down;

            haze_anim.SetBool("isCrouching_anim", true);

        }
        else if (Input.GetButtonDown("Crouch") && isGrounded == true && canMove == true && isCrouched == true)
        {
            GetUpFromCrouch();
        }
    }
}
