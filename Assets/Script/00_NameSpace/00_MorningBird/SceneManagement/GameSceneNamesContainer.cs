using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MorningBird.SceneManagement
{
    public class GameSceneNamesContainer : SerializedMonoBehaviour
    {
        #region Singleton Instance

        static GameSceneNamesContainer gameSceneNamesContainer;
        /// <summary>
        /// GameSceneNamesContainer
        /// </summary>
        public static GameSceneNamesContainer GSNC
        {
            get
            {
                if (gameSceneNamesContainer == null)
                {
                    var obj = FindObjectOfType<GameSceneNamesContainer>();

                    if (obj != null)
                    {
                        gameSceneNamesContainer = obj;
                    }
                    else
                    {
                        var newSingleton = GameObject.Find("TotalGameController").GetComponent<GameSceneNamesContainer>();
                        gameSceneNamesContainer = newSingleton;
                    }
                }
                return gameSceneNamesContainer;
            }
        }

        private void Awake()
        {
            var obj = FindObjectsOfType<GameSceneNamesContainer>();
            if (obj.Length > 1)
            {
                Destroy(this);
                Debug.LogError("GameSceneNamesContainer :: instantiate another singleton object. So Destory instance");
            }
        }

        #endregion

        [System.Serializable]
        struct GameObjects
        {
            public GameObject[] objs;
        }

        [SerializeField] Dictionary<string, GameObjects> gameSceneLoaderStringPairs = new Dictionary<string, GameObjects>();

        public bool TryGetSceneLoadGO(string sceneKey, int sceneNumber, out GameObject nullAbleSceneLoader)
        {
            GameObjects sceneList;
            bool isKeyExist = gameSceneLoaderStringPairs.TryGetValue(sceneKey, out sceneList);

            GameObject returnObject = null;
            try
            {
                returnObject = sceneList.objs[sceneNumber];
            }
            catch(System.IndexOutOfRangeException)
            {
                returnObject = null;
            }


            nullAbleSceneLoader = returnObject;
            if(nullAbleSceneLoader == null)
            {
                isKeyExist = false;
            }

            return isKeyExist;
        }
    }

}

