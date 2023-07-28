using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MorningBird.SceneManagement
{
    public class GroupSceneLoader : SerializedMonoBehaviour
    {
        [SerializeField] string sceneBody = null;
        [SerializeField] bool addSuffixNumber = false;
        [SerializeField, ShowIf("addSuffixNumber", true)] int suffixNumber = 0;
        [SerializeField] string sceneMain = null;

        [SerializeField] bool _useSubScenes = false;
        [SerializeField, ShowIf("_useSubScenes", true)] string[] subScenes;
        [SerializeField] bool useAdditionalScenes = false;
        [SerializeField, ShowIf("useAdditionalScenes", true)] string[] AdditionalScenes;

        [SerializeField] string _dotsSubScene;

        private void Start()
        {
            StartCoroutine(WaitOneFrameAndLoad());
        }

        public void LoadScene()
        {
            if (sceneMain.Length < 3 || sceneBody.Length < 3)
            {
                return;
            }
            else
            {
                // wait for second
            }

            InnerSceneLoading();
        }

        private void InnerSceneLoading()
        {
            // Declrare array
            List<string> sceneList = new();

            if (addSuffixNumber == true)
            {
                // Add MainScene
                sceneList.Add($"{sceneBody}_{suffixNumber}_{sceneMain}");

                if (_useSubScenes == true)
                {
                    // Add SubScenes
                    if (subScenes.Length > 0)
                    {
                        for (int i = 0; i < subScenes.Length; i++)
                        {
                            var subName = subScenes[i];

                            sceneList.Add($"{sceneBody}_{suffixNumber}_{subName}");
                        }
                    }
                }
            }
            else
            {
                // Add MainScene
                sceneList.Add($"{sceneBody}_{sceneMain}");

                if (_useSubScenes == true)
                {
                    // Add SubScenes
                    if (subScenes.Length > 0)
                    {
                        for (int i = 0; i < subScenes.Length; i++)
                        {
                            var subName = subScenes[i];

                            sceneList.Add($"{sceneBody}_{subName}");
                        }
                    }
                }
            }

            // Add AdditionalScene
            if (useAdditionalScenes == true)
            {
                foreach (string name in AdditionalScenes)
                {
                    sceneList.Add(name);
                }
            }

            // LoadScene
            if (sceneList.Count == 1)
            {
                GameSceneLoadManager.GSLM.LoadSceneAsync(sceneList[0]);
            }
            else
            {
                GameSceneLoadManager.GSLM.LoadSceneAsync(sceneList.ToArray());
            }
        }

        IEnumerator WaitOneFrameAndLoad()
        {
            if (sceneMain.Length < 3 || sceneBody.Length < 3)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }

            InnerSceneLoading();

            Destroy(this.gameObject);
        }
    }
}