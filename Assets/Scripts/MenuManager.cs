using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> images = new List<GameObject>();
    [SerializeField] private int slideDuration = 3;
    private int i = 0;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        images[i].SetActive(true);
        StartCoroutine(NextSlide());
    }

    public void NextImage()
    {
        images[i].SetActive(false);
        if(i < images.Count - 1)
        {
            i++;
            images[i].SetActive(true);
            StartCoroutine(NextSlide());
        }
        else
        {
            // Load Scene ???
            gameObject.SetActive(false);
        }
    }

    IEnumerator NextSlide()
    {
        yield return new WaitForSeconds(slideDuration);
        NextImage();
    }
}
