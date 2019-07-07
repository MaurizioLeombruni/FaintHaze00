using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagement : MonoBehaviour
{
    //Referenze ai pannelli che contengono i controlli e i crediti, per attivarli quando necessario.
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    //Referenza all'animazione dei crediti, necessaria per gestirla.
    public Animator creditsAnimation;

    //Una booleana di supporto che impedisce al giocatore di accedere ad altri pulsanti se si ha attivato un pannello.
    private bool panel_active;

    public void StartButton()
    {
        if(!panel_active)
            SceneManager.LoadScene("Scena1");
    }

    public void LoadButton()
    {
        if(!panel_active)
            SceneManager.LoadScene("LoadLevels");
    }

    public void ControlsButton()
    {
        if (!panel_active)
        {
            panel_active = true;
            controlsPanel.SetActive(true);
        }

    }

    public void ExitControls()
    {
        controlsPanel.SetActive(false);
        panel_active = false;
    }

    public void CreditsButton()
    {
        if (!panel_active)
        {
            panel_active = true;
            creditsPanel.SetActive(true);
            creditsAnimation.SetTrigger("credits");
        }
    }

    public void ExitCredits()
    {
        creditsPanel.SetActive(false);
        panel_active = false;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel_active)
            {
                ExitControls();
                ExitCredits();
            }
        }
    }
}
