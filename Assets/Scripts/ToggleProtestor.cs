using Game.NPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleProtestor : MonoBehaviour
{
    [SerializeField] private GameObject sign, flower, sadFace, happyFace;

    public void OnToggleProtestor(ECivilionState state)
    {
        if (state == ECivilionState.Peace)
        {
            sign.SetActive(false);
            flower.SetActive(true);
            sadFace.SetActive(false);
            happyFace.SetActive(true);
        } else
        {
            sign.SetActive(true);
            flower.SetActive(false);
            sadFace.SetActive(true);
            happyFace.SetActive(false);
        }
    }
}
