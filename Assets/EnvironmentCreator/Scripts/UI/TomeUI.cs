using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomeUI : MonoBehaviour
{
    //If you want to personalize the UI, this is the script to do it!
    //You can edit the text, make it play a SFX when enabled, tween the UI in and out, etc. It would all be done here!

    private void OnEnable()
    {
        //If anything should happen when the UI is enabled
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
        PauseManager.FreezeAllMovingObject(false); //Unfreeze NPCs and Players
    }
}
