using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntangibilityEffect : MonoBehaviour
{
    //Il seguente script altera l'aspetto dell'oggetto in cui si nasconde il personaggio, aggiungendo trasparenza? al fine di
    //far vedere la posizione esatta al suo interno.

    public GameObject pippo;

    public Material mat_intangible;
    private Material mat_original;

    private void Awake()
    {
        mat_original = pippo.GetComponent<MeshRenderer>().material;
    }

    public void ChangeVision()
    {
        pippo.GetComponent<MeshRenderer>().material = mat_intangible;
    }

    public void ChangeBack()
    {
        pippo.GetComponent<MeshRenderer>().material = mat_original;
    }

    public virtual void PushAway(GameObject target)
    {

        Debug.Log("PUSH!");
        if (target.GetComponent<BoxCollider2D>().bounds.center.x < pippo.GetComponent<BoxCollider2D>().bounds.center.x)
        {
            target.transform.position = Vector2.Lerp(target.transform.position, target.transform.position + new Vector3(-3, 0, 0), 0.75f);
        }
        else
        {
            target.transform.position = Vector2.Lerp(target.transform.position, target.transform.position + new Vector3(3, 0, 0), 0.75f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<Haze>().isIntangible == true)
            {
                ChangeVision();
                collision.GetComponent<Haze>().isStuck = true;
            }

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.GetComponent<Haze>().isStuck == true)
        {
            if (collision.GetComponent<Haze>().isIntangible == false)
            {
                collision.GetComponent<Haze>().isStuck = false;
                PushAway(collision.gameObject);
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ChangeBack();
            collision.GetComponent<Haze>().isStuck = false;
        }
    }
}
