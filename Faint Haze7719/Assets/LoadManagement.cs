using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManagement : MonoBehaviour
{
    //Lo script gestisce i livelli da caricare.
    //Mostra solo quelli sbloccati dal giocatore, e al click del rispettivo tasto carica il livello designato.

    //Referenze ai pulsanti nascosti. I pulsanti dei livelli non sbloccati sono disattivati di default.
    public GameObject load_01;
    public GameObject load_02;
    public GameObject load_03;
    public GameObject load_04;

    //Il valore massimo della scena sbloccata. Più è alto il valore, più scene verrano sbloccate.
    public int unlocks;

    public void BackButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevelOne()
    {
        SceneManager.LoadScene("Scena1");
    }

    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("Scena2");
    }

    public void LoadLevelThree()
    {
        SceneManager.LoadScene("Scena3");
    }

    public void LoadLevelFour()
    {
        SceneManager.LoadScene("Scena4");
    }

    public void LoadLevelFive()
    {
        SceneManager.LoadScene("Scena5");
    }

    public void CheckLevelUnlock()
    {
        unlocks = PlayerPrefs.GetInt("levels");
        if (unlocks >= 1)
        {
            load_01.SetActive(true);
        }
        if (unlocks >= 2)
        {
            load_02.SetActive(true);
        }
        if (unlocks >= 3)
        {
            load_03.SetActive(true);
        }
        if (unlocks >= 4)
        {
            load_04.SetActive(true);
        }
    }
}
