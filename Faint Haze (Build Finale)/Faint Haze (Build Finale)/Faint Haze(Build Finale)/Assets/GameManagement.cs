using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    //Il Game Manager tiene conto di tutti i valori comuni a tutte le scene, come il numero di chiavi prese
    //o la variabile booleana che controlla il filtro dell'intangibilità.
    //Tutti i valori sono public in quanto vengono usati da più script all'interno del gioco.

    //Questi valori interi vengono utilizzati dalle porte per capire se il giocatore può passare o meno.
    //Il primo valore viene modificato in base alle esigenze del livello, e gestisce il numero di chiavi necessarie per passare.
    //Il secondo valore è un valore interno con cui il gioco tiene traccia di quante chiavi il giocatore ha trovato.
    public int keysNeeded;
    public int keysFound;

    //Questo valore booleano attiva il filtro dell'intangibilità. Tutti gli oggetti che usano questo filtro cambiano colore e/o trasparenza al fine
    //di mettere in risalto gli effetti dell'intangibilità.
    public bool filterActive;

    //La referenza al filtro su schermo dell'intangibilità. La referenza è pubblica per evitare bug con il ritrovamento del filtro, poiché esso parte
    //disattivato dalla scena.
    public GameObject filterEffect;

    //Referenza ai pannelli di pausa e Game Over.
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    public bool pauseActive;

    //Il nome della scena.
    public string thisScene;

    public void RetryButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(thisScene);
    }

    public void MenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void ReturnButton()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    private void Awake()
    {
        //Setta a 0 le chiavi trovate ad ogni caricamento di scena.
        keysFound = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseActive)
            {
                pauseActive = true;
                pausePanel.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                pauseActive = false;
                pausePanel.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    private void FixedUpdate()
    {

        //Attiva o disattiva il filtro dell'intangibilità a seconda se la skill è attiva o meno.
        if (filterActive)
        {
            filterEffect.SetActive(true);
        }
        else
        {
            filterEffect.SetActive(false);
        }
    }
}
