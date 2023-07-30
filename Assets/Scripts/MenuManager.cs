using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> images = new List<GameObject>();
    [SerializeField] private int slideDuration = 3;
    [SerializeField] private GameObject _gameSceneLoader;
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
            Instantiate(_gameSceneLoader);
        }
    }

    IEnumerator NextSlide()
    {
        yield return new WaitForSeconds(slideDuration);
        NextImage();
    }
}
