using System.Collections;
using UnityEngine;

namespace MorningBird.SceneManagement
{
    public class GeneralSceneLoader : MonoBehaviour
    {
        [SerializeField] string[] sceneNames;
        [SerializeField] float _waitTime;
        [SerializeField] bool _requestUnloadAllScenes;

        // Use this for initialization
        void Start()
        {
            if (sceneNames.Length == 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (sceneNames[0].Length < 3)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    StartCoroutine(WaintOneFrameAndLoad());
                }
            }
        }

        public void LoadScene()
        {

            if (sceneNames.Length == 0)
                return;

            if (sceneNames[0].Length < 3)
                return;

            GameSceneLoadManager.GSLM.LoadSceneAsync(sceneNames);
        }

        IEnumerator WaintOneFrameAndLoad()
        {
            yield return new WaitForSecondsRealtime(_waitTime);
            yield return new WaitForEndOfFrame();
            if(_requestUnloadAllScenes == true)
            {
                GameSceneLoadManager.GSLM.UnLoadAllScenes();
            }

            GameSceneLoadManager.GSLM.LoadSceneAsync(sceneNames);

            Destroy(this.gameObject);
        }
    }
}