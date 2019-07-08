using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip Salto, SuonoMorte, RampinoAggancio, AttaccoNemico, CartaMagnetica, FeromoniIntNemici, FeromoniLancio, FeromoniRotti, IntangibilitaAttivazione, IntangibilitaDisattivazione;
    static AudioSource audioSrc;

    private void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Salto = Resources.Load<AudioClip>("Salto");
        SuonoMorte = Resources.Load<AudioClip>("SuonoMorte");
        RampinoAggancio = Resources.Load<AudioClip>("RampinoAggancio");
        AttaccoNemico = Resources.Load<AudioClip>("AttaccoNemico");
        CartaMagnetica = Resources.Load<AudioClip>("CartaMagnetica");
        FeromoniIntNemici = Resources.Load<AudioClip>("FeromoniIntNemici");
        FeromoniLancio = Resources.Load<AudioClip>("FeromoniLancio");
        FeromoniRotti = Resources.Load<AudioClip>("FeromoniRotti");
        IntangibilitaAttivazione = Resources.Load<AudioClip>("IntangibilitaAttivazione");
        IntangibilitaDisattivazione = Resources.Load<AudioClip>("IntangibilitaDisattivazione");

        audioSrc = GetComponent<AudioSource>();
    }


    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Salto":
                audioSrc.PlayOneShot(Salto);
                break;
            case "SuonoMorte":
                audioSrc.PlayOneShot(SuonoMorte);
                break;
            case "RampinoAggancio":
                audioSrc.PlayOneShot(RampinoAggancio);
                break;
            case "AttaccoNemico":
                audioSrc.PlayOneShot(AttaccoNemico);
                break;
            case "CartaMagnetica":
                audioSrc.PlayOneShot(CartaMagnetica);
                break;
            case "FeromoniIntNemici":
                audioSrc.PlayOneShot(FeromoniIntNemici);
                break;
            case "FeromoniLancio":
                audioSrc.PlayOneShot(FeromoniLancio);
                break;
            case "FeromoniRotti":
                audioSrc.PlayOneShot(FeromoniRotti);
                break;
            case "IntangibilitaAttivazione":
                audioSrc.PlayOneShot(IntangibilitaAttivazione);
                break;
            case "IntangibilitaDisattivazione":
                audioSrc.PlayOneShot(IntangibilitaDisattivazione);
                break;
        }

    }
}
