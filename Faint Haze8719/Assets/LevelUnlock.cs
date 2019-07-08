using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnlock : MonoBehaviour
{
    //Sblocca il livello specificato una volta completato.

    public int level;

    public void UnlockLevel()
    {
        if(PlayerPrefs.GetInt("levels") < level)
            PlayerPrefs.SetInt("levels", level);
    }
}
