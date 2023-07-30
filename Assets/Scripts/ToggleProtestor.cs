using Game.NPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleProtestor : MonoBehaviour
{
    [SerializeField] private GameObject sign, sign2, flower, sadFace, happyFace;

    // GetComponentInChildren<ToggleProtestor>().OnToggleProtestor(state);
    public void OnToggleProtestor(ECivilionState state)
    {
        if (state == ECivilionState.Peace)
        {
            if(sign2 != null)
                sign2.SetActive(false);
            sign.SetActive(false);
            flower.SetActive(true);
            sadFace.SetActive(false);
            happyFace.SetActive(true);
        } else
        {
            if (sign2 != null)
                sign2?.SetActive(true);
            sign.SetActive(true);
            flower.SetActive(false);
            sadFace.SetActive(true);
            happyFace.SetActive(false);
        }

    }
}
