using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeta : MonoBehaviour
{
    //La durata degli stati di Beta; quanto tempo rimane nel suo stato di veglia o di sonno.
    //I valori possono essere diversi per consentire una maggiore libertà nel level design.
    public int sleep_duration;
    public int wake_duration;
    public GameObject conoDiVisione;

    //Lo stato di Beta; se sta guardando o se sta dormendo.
    private bool watching;

    //La coroutine che cambia lo stato di Beta nel tempo.
    IEnumerator Sveglia()
    {
        while (true)
        {
            yield return new WaitForSeconds(wake_duration);
            watching = false;
            conoDiVisione.SetActive(false);
            Debug.Log("Beta va a dormire");

            yield return new WaitForSeconds(sleep_duration);
            watching = true;
            conoDiVisione.SetActive(true);
            Debug.Log("Beta è sveglio");
        }
        
    }

    private void Start()
    {
        sleep_duration = 5;
        wake_duration = 4;
        watching = true;
        StartCoroutine("Sveglia");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Haze>())
        {
            if (watching)
            {
                if(collision.GetComponent<Haze>().stealth_status == Haze.Visibility.Visible || collision.GetComponent<Haze>().stealth_status == Haze.Visibility.Caution)
                {
                    collision.GetComponent<Haze>().Death();
                }

            }
        }

    }

}
