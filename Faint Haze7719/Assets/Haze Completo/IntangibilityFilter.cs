using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntangibilityFilter : MonoBehaviour
{
    //Il seguente script controlla l'aspetto grafico del filtro dell'intangibilità, riferito ai nascondigli.

    private GameManagement manager;

    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("manager").GetComponent<GameManagement>();
    }

    private void FixedUpdate()
    {
        if (manager.filterActive)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
