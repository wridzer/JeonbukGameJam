using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using MorningBird.UI;
using Sirenix.OdinInspector;

namespace MorningBird.SceneManagement
{
    public class GameSceneLoadManager : SerializedMonoBehaviour
    {
        [Title("BasicSettings"), SerializeField] string[] _preserveSceneList;
        Scene _totalGameControllScene;

        #region Singleton
        
        static GameSceneLoadManager _gameSceneManager;
        /// <summary>
        /// TotalGameSceneManager
        /// </summary>
        internal static GameSceneLoadManager GSLM
        {
            get
            {
                if (_gameSceneManager == null)
                {
                    var obj = FindObjectOfType<GameSceneLoadManager>();

                    if (obj != null)
                    {
                        _gameSceneManager = obj;
                    }
                    else
                    {
                        var newSingleton = GameObject.Find("GameSceneLoadManager").GetComponent<GameSceneLoadManager>();
                        _gameSceneManager = newSingleton;
                    }
                }
                return _gameSceneManager;
            }
        }

        #endregion

        #region TestSettings
#if UNITY_EDITOR
        [FoldoutGroup("Test Settings_It will disable when game is build"), SerializeField] bool _testLoading;
        [FoldoutGroup("Test Settings_It will disable when game is build"), SerializeField] bool _testUnLoading;
        [FoldoutGroup("Test Settings_It will disable when game is build"), SerializeField] string[] _testStringArr;
#endif
        #endregion

        #region Conditions And Debug Variables

        [FoldoutGroup("Conditions"), SerializeField] List<AsyncOperation> sceneLoadingList = new List<AsyncOperation>();
#pragma warning disable CS0414 // 사용되지 않은 변수에 대한 경고 끄기
        [FoldoutGroup("Conditions"), SerializeField] bool _isAllScenesLoaded;
#pragma warning restore CS0414 // 사용되지 않은 변수에 대한 경고 끄기
        //[FoldoutGroup("DebugVariables"), SerializeField] CinematicControllManager_Common _cinematicControllM;
        //[FoldoutGroup("DebugVariables"), SerializeField] LoadingScreenShowerManager _loadingScreenM;
        
        #endregion


        bool _isLoadingScreen = false;

        private void Awake()
        {
            AwakeInitialize();
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (_testLoading)
            {
                _testLoading = false;
                LoadSceneAsync(_testStringArr);
            }

            if (_testUnLoading)
            {
                _testUnLoading = false;
                UnLoadSceneAsync(_testStringArr);
            }
#endif
        }

        void AwakeInitialize()
        {
            // Find this Manager Scene
            {
                _totalGameControllScene = this.gameObject.scene;
            }

        }

        void SetSceneLoadBoolToFalse()
        {
            _isAllScenesLoaded = false;
            if (_isLoadingScreen == true)
            {
                InitializeLoadingScreen();
            }

            void InitializeLoadingScreen()
            {
                //_loadingScreenM.TurnOnLoadingScreen(5f);
            }
        }

        void SetSceneLoadBoolToTrue()
        {
            _isAllScenesLoaded = true;
            DeInitializeLoadingScreen();

            void DeInitializeLoadingScreen()
            {
                _isLoadingScreen = false;
                // _loadingScreenM.TurnOffLoadingScreen();
            }
        }

        Scene[] GetCurrentOpenedScenes()
        {
            Scene[] scenes = new Scene[SceneManager.sceneCount];

            // GetCurrentOpenedScenes
            {
                for (int ia = 0; ia < scenes.Length; ia++)
                {
                    scenes[ia] = SceneManager.GetSceneAt(ia);
                }
            }
            return scenes;
        }

        public void LoadSceneAsync(string sceneName, bool isLoadingScreenOn = false)
        {
            sceneLoadingList.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));

            _isLoadingScreen = isLoadingScreenOn;

            SetSceneLoadBoolToFalse();
            StartCoroutine(WaitUntilScenesLoadedAndSetActiveScene(sceneName));
        }

        public void LoadSceneAsync(string[] sceneName, bool isLoadingScreenOn = false)
        {
            for (int i = 0; i < sceneName.Length; i++)
            {
                var name = sceneName[i];
                sceneLoadingList.Add(SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive));
            }


            _isLoadingScreen = isLoadingScreenOn;

            SetSceneLoadBoolToFalse();
            StartCoroutine(WaitUntilScenesLoadedAndSetActiveScene(sceneName[0]));
        }

        public void UnLoadSceneAsync(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            SetSceneLoadBoolToFalse();
        }

        public void UnLoadSceneAsync(string[] sceneName)
        {
            foreach (var scene in sceneName)
            {
                SceneManager.UnloadSceneAsync(scene);
            }

            SetSceneLoadBoolToFalse();
        }

        public void UnLoadAllSceneAsyncAndLoadSceneAsync(string sceneName, string[] exceptingScene = null)
        {
            UnLoadAllScenes(exceptingScene);
            LoadSceneAsync(sceneName);
        }

        public void UnLoadAllScenes(string[] exceptingScene = null)
        {
            Scene[] scenes = GetCurrentOpenedScenes();

            List<string> sceneList = new List<string>();

            foreach (Scene scene in scenes)
            {
                sceneList.Add(scene.name);
            }

            sceneList.Remove(_totalGameControllScene.name);

            if(exceptingScene != null)
            {
                foreach (var name in exceptingScene)
                {
                    sceneList.Remove(name);
                }
            }


            foreach (var name in _preserveSceneList)
            {
                sceneList.Remove(name);
            }

            UnLoadSceneAsync(sceneList.ToArray());
        }

        public void UnLoadAllSceneAsyncAndLoadSceneAsync(GroupSceneLoader sceneLoaderClass)
        {
            UnLoadAllScenes();
            sceneLoaderClass.LoadScene();
        }

        public void UnLoadAllSceneAsyncAndLoadSceneAsync(GeneralSceneLoader sceneLoaderClass)
        {
            UnLoadAllScenes();
            sceneLoaderClass.LoadScene();
        }

        public void SetCenterOfScenes(string sceneName)
        {
            Scene[] scenes = new Scene[SceneManager.sceneCount];

            // GetCurrentOpenedScenes
            {
                for (int ia = 0; ia < scenes.Length; ia++)
                {
                    scenes[ia] = SceneManager.GetSceneAt(ia);
                }
            }

            // Find sceneName For Set Active Scene.
            for (int ia = 0; ia < scenes.Length; ia++)
            {
                if (scenes[ia].name == sceneName)
                {
                    SceneManager.SetActiveScene(scenes[ia]);
                    return;
                }
            }

            Debug.LogError("Check SceneName. Set active scene is failed.");
            return;
        }

        IEnumerator WaitUntilScenesLoadedAndSetActiveScene(string sceneName)
        {
            while (sceneLoadingList.Count > 0)
            {

#if UNITY_EDITOR
                Debug.Log($"SceneLoading Screen Turn On");
                Debug.Log($"SceneLoading Screen keeping On");
#endif

                yield return new WaitForSeconds(0.25f);

                float totalRatioOfPregress = new();
                int preserveScenesCount = sceneLoadingList.Count;
                for (int i = 0; i < sceneLoadingList.Count; i++)
                {
                    AsyncOperation operation = sceneLoadingList[i];
                    totalRatioOfPregress += operation.progress;

                    if (operation.isDone == true)
                    {
                        sceneLoadingList.Remove(operation);
                    }
                }

                // Check Total Progress
                if(sceneLoadingList.Count == 0)
                {
                    totalRatioOfPregress = 1;
                }
                else
                {
                    totalRatioOfPregress = totalRatioOfPregress / preserveScenesCount;
                }
#if UNITY_EDITOR
                Debug.Log($"Scene Loading {totalRatioOfPregress} Complite.");
#endif
            }
#if UNITY_EDITOR
            Debug.Log($"SceneLoading Screen Turn Off");
#endif
            SetSceneLoadBoolToTrue();
            sceneLoadingList.Clear();
            SetCenterOfScenes(sceneName);

        }
    }
}
