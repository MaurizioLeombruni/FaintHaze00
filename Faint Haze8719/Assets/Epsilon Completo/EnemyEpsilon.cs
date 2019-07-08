using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEpsilon : MonoBehaviour
{
    //Reference al collider dell'oggetto. Usato per accedere al raggio di detezione del player, per poterlo modificare in seguito
    //all'effetto di un'interazione ambientale.
    private CircleCollider2D detection;

    //Variabili di controllo per l'aspetto grafico & di animazione del nemico.
    public GameObject circle_normal;
    public GameObject circle_small;

    public Animator epsilon_anim;

    //Valori numeri utilizzati nella modifica del raggio di detezione.
    public float radius_initial;
    public float radius_modified;

    //Valori numerici di supporto.
    public float reduction_duration;

    private void Awake()
    {
        detection = GetComponent<CircleCollider2D>();
    }

    //Setta i valori iniziali del nemico.

    private void Start()
    {
        radius_initial = 1.67f;
        radius_modified = 1.0f;
    }

    IEnumerator ReduceDetection()
    {
        //Modifica il raggio di detenzione del nemico secondo i valori specificati. Dopodiché modifica appositamente l'aspetto grafico.
        detection.radius = radius_modified;

        circle_normal.SetActive(false);
        circle_small.SetActive(true);

        epsilon_anim.SetTrigger("reduce_anim");

        yield return new WaitForSeconds(reduction_duration);

        detection.radius = radius_initial;

        circle_normal.SetActive(true);
        circle_small.SetActive(false);

        epsilon_anim.SetTrigger("cease_anim");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Haze>().Death();
        }
    }

}
