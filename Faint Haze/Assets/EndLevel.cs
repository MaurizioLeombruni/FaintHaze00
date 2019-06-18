using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public string scenaSeguente;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Aaalllala");
                SceneManager.LoadScene("SceneEpsilon");
            }
            
        }
    }

}
