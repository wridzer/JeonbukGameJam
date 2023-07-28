
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MorningBird.Sound
{
    public enum SoundPoolTag
    {
        Clip,
        Ambience,
        BGM
    }

    public class SoundManager : MonoBehaviour
    {
        #region Singleton Settings

        private static SoundManager _soundManager;

        /// <summary>
        /// SoundManager
        /// </summary>
        public static SoundManager Instance
        {
            get
            {
                if (_soundManager == null)
                {
                    SoundManager obj = FindObjectOfType<SoundManager>();

                    if (obj != null)
                    {
                        _soundManager = obj;
                    }
                    else
                    {
                        var newSingleton = new GameObject("SoundManager").AddComponent<SoundManager>();
                        _soundManager = newSingleton;
                    }
                }
                return _soundManager;
            }
        }

        #endregion

        private void Awake()
        {
            #region Singleton Instantiate
            var objs = FindObjectsOfType<SoundManager>();
            if (objs.Length > 1)
            {
                Debug.LogError("New SoundManager Added And Destroy Automatically");
                Destroy(this.gameObject);
                return;
            }
            #endregion
        }

        #region Variables

        [SerializeField, Range(0, 1.1f)] float _bgmVolume = 1.0f;
        [SerializeField, Range(0, 1.1f)] float _ambientVolume = 1.0f;
        [SerializeField, Range(0, 1.1f)] float _soundClipVolume = 1.0f;

        public float SetBGMVolume
        {
            set
            {
                float t_value = Mathf.Clamp(value, 0.0f, 1.1f);
                _bgmVolume = t_value;
                UpdateBGMVolume();
            }
        }

        public int SetBGMIntVolume
        {
            set
            {
                float t_value = value * 0.01f;
                SetBGMVolume = t_value;
            }
        }

        public void SetBGMSliderVolume(Slider slider)
        {
            SetBGMVolume = slider.value;
        }

        public void SetBGMSliderIntVolume(Slider slider)
        {
            SetBGMIntVolume = (int)slider.value;
        }

        public float GetBGMVolume => _bgmVolume;

        public float SetAmbientVolume
        {
            set
            {
                float t_value = Mathf.Clamp(value, 0.0f, 1.1f);
                _ambientVolume = t_value;
                UpdateAmbientVolume();
            }
        }

        public int SetAmbientIntVolume
        {
            set
            {
                float t_value = value * 0.01f;
                SetAmbientVolume = t_value;
            }
        }

        public void SetAmbientSliderVolume(Slider slider)
        {
            SetAmbientVolume = slider.value;
        }

        public void SetAmbientSliderIntVolume(Slider slider)
        {
            SetAmbientIntVolume = (int)slider.value;
        }

        public float GetAmbientVolume => _ambientVolume;

        public float SetSoundClipVolume
        {
            set
            {
                float t_value = Mathf.Clamp(value, 0.0f, 1.1f);
                _soundClipVolume = t_value;
            }
        }

        public int SetSoundClipIntVolume
        {
            set
            {
                float t_value = value * 0.01f;
                SetSoundClipVolume = t_value;
            }
        }

        public void SetSoundClipSliderVolume(Slider slider)
        {
            SetSoundClipVolume = slider.value;
        }

        public void SetSoundClipSliderIntVolume(Slider slider)
        {
            SetSoundClipIntVolume = (int)slider.value;
        }

        public float GetSoundClipVolume => _soundClipVolume;

        #region Basic Variables

        [SerializeField] AudioStorage[] _BGMS;
#if (UNITY_EDITOR)
        [SerializeField] bool _test;
#endif
        [SerializeField] AudioStorage _testAudioStorage;
        [SerializeField] Vector3 _testAudioStoragePos = Vector3.zero;

        [SerializeField] GameObject _objectPoolingPrefab;

        [SerializeField] GameObject _BGMPool, _ambiencePool, _clipPool;

        [SerializeField] int _currentBGMNumber;
        [SerializeField] int _currentAmbienceNumber;
        [SerializeField] int _currentClipNumber;

        [SerializeField] int _howManyUseBGMSoundObjects = 4;
        int _numberOfBGMSOundObjects => _howManyUseBGMSoundObjects;
        [SerializeField] GameObject[] _BGMSoundObjectsPool;

        [SerializeField] int _howManyUseAmbienceSoundObjects = 32;
        int _numberOfAmbienceSoundObjects => _howManyUseAmbienceSoundObjects;
        [SerializeField] GameObject[] _ambienceSoundObjectsPool;

        [SerializeField] int _howManyUseClipSoundObjects = 128;
        int _numberOfClipSoundObjects => _howManyUseClipSoundObjects;
        [SerializeField] GameObject[] _clipSoundObjectsPool;

        #endregion

        #endregion


        private void Start()
        {
            Initialization();
        }

        #region Initializations

        void Initialization()
        {
            InstantiateClipacks();
        }

        void InstantiateClipacks()
        {
            _BGMPool = new GameObject();
            _ambiencePool = new GameObject();
            _clipPool = new GameObject();

            _BGMPool.name = "BGMPool";
            _ambiencePool.name = "AmbiencePool";
            _clipPool.name = "ClipPool";

            _BGMPool.transform.parent = this.gameObject.transform;
            _ambiencePool.transform.parent = this.gameObject.transform;
            _clipPool.transform.parent = this.gameObject.transform;

            _BGMSoundObjectsPool = new GameObject[_numberOfBGMSOundObjects];
            _BGMSoundObjectsPool = InstantiateSoundPoolingGameObjects(_numberOfBGMSOundObjects, _BGMPool.transform);


            _ambienceSoundObjectsPool = new GameObject[_numberOfAmbienceSoundObjects];
            _ambienceSoundObjectsPool = InstantiateSoundPoolingGameObjects(_numberOfAmbienceSoundObjects, _ambiencePool.transform);

            // instantiate soundclip
            _clipSoundObjectsPool = new GameObject[_numberOfClipSoundObjects];
            _clipSoundObjectsPool = InstantiateSoundPoolingGameObjects(_numberOfClipSoundObjects, _clipPool.transform);

            // Set sound pool tag
            SetSoundPoolTag(_BGMSoundObjectsPool, _numberOfBGMSOundObjects, SoundPoolTag.BGM);
            SetSoundPoolTag(_ambienceSoundObjectsPool, _numberOfAmbienceSoundObjects, SoundPoolTag.Ambience);
            SetSoundPoolTag(_clipSoundObjectsPool, _numberOfClipSoundObjects, SoundPoolTag.Clip);
        }

        internal void ReInitializeClipPack()
        {
            Destroy(_BGMPool.gameObject);
            Destroy(_ambiencePool.gameObject);
            Destroy(_clipPool.gameObject);
            InstantiateClipacks();
        }

        void SetSoundPoolTag(GameObject[] whatGameObjects, int howMany, SoundPoolTag tag)
        {
            for (int ai = 0; ai < howMany; ai++)
            {
                whatGameObjects[ai].GetComponent<BasicSoundClipPlay_Common>().SetSoundPoolTag(tag);
            }
        }

        GameObject[] InstantiateSoundPoolingGameObjects(int howMany, Transform setParent)
        {
            GameObject[] gameObjects = new GameObject[howMany];
            for (int ia = 0; ia < howMany; ia++)
            {
                GameObject a = Instantiate(_objectPoolingPrefab, setParent);
                a.name = ia.ToString();
                gameObjects[ia] = a;
            }
            return gameObjects;
        }

        #endregion

        [SerializeField] bool _reInitializeClipPack;

        private void FixedUpdate()
        {
#if (UNITY_EDITOR)
            if (_test)
            {
                RequestPlayClip(_testAudioStorage, _testAudioStoragePos);
                _test = false;
            }

            if (_reInitializeClipPack)
            {
                ReInitializeClipPack();
                _reInitializeClipPack = false;
            }
#endif
        }

        #region UpdateVolume

        void UpdateBGMVolume()
        {
            for (int i = 0; i < _BGMPool.transform.childCount; i++)
            {
                var basicSoundClipCommon = _BGMPool.transform.GetChild(i).GetComponent<BasicSoundClipPlay_Common>();
                var auidoStorage = basicSoundClipCommon.GetAudioStorage;
                float storageVolume;
                if (auidoStorage == null)
                {
                    storageVolume = 0f;
                }
                else
                {
                    storageVolume = auidoStorage.Volume;
                }

                basicSoundClipCommon.SetAudioVolume(_bgmVolume * storageVolume);
            }
        }

        void UpdateAmbientVolume()
        {
            for (int i = 0; i < _ambiencePool.transform.childCount; i++)
            {
                var basicSoundClipCommon = _ambiencePool.transform.GetChild(i).GetComponent<BasicSoundClipPlay_Common>();

                var auidoStorage = basicSoundClipCommon.GetAudioStorage;
                float storageVolume;
                if (auidoStorage == null)
                {
                    storageVolume = 0f;
                }
                else
                {
                    storageVolume = auidoStorage.Volume;
                }


                basicSoundClipCommon.SetAudioVolume(_ambientVolume * storageVolume);
            }
        }

        void UpdateSoundClipVolume()
        {
            for (int i = 0; i < _clipPool.transform.childCount; i++)
            {
                var basicSoundClipCommon = _clipPool.transform.GetChild(i).GetComponent<BasicSoundClipPlay_Common>();
                var auidoStorage = basicSoundClipCommon.GetAudioStorage;
                float storageVolume;
                if (auidoStorage == null)
                {
                    storageVolume = 0f;
                }
                else
                {
                    storageVolume = basicSoundClipCommon.GetAudioStorage.Volume;
                }

                basicSoundClipCommon.SetAudioVolume(_soundClipVolume * storageVolume);
            }
        }

        #endregion

        #region Request BGM

        /// <summary>
        /// Stop BGM. It will Just stop all BGM.
        /// </summary>
        internal void RequestStopAllBGM()
        {
            foreach (Transform target in _BGMPool.GetComponentInChildren<Transform>())
            {
                if (target.gameObject.activeSelf)
                {
                    ClipRequestReturnToPool(target);
                }
            }
        }

        /// <summary>
        /// Turn off target BGM. This function will stop "bgm"name BGM in SoundManager. If bgm isn't play, not gonna stop for any bgm.
        /// </summary>
        /// <param name="bgm"></param>
        /// <param name="notStopSameName">If it turn to true, "bgm"name BGM in SoundManager not stop. Instead, rest of BGM's will Stop.</param>
        internal void RequestStopTheRestBGM(AudioStorage bgm, out bool isNecessaryForPlayBGM)
        {
            isNecessaryForPlayBGM = true;

            isNecessaryForPlayBGM = FindUnsameNameOfBGMAndStop(bgm, isNecessaryForPlayBGM);

            bool FindUnsameNameOfBGMAndStop(AudioStorage bgm, bool isNecessaryForPlayBGM)
            {
                foreach (Transform target in _BGMPool.GetComponentInChildren<Transform>())
                {
                    #region Local Variables

                    bool isSetActiveBGMTarget = target.gameObject.activeInHierarchy;
                    bool isSameBetweenTargetNameToStorageName = target.gameObject.name == bgm.Name;

                    #endregion

                    if (isSetActiveBGMTarget == false)
                        continue;

                    if (isSameBetweenTargetNameToStorageName)
                    {
                        isNecessaryForPlayBGM = false;
                    }
                    else
                    {
                        StopUnsameNameBGM(target);
                        isNecessaryForPlayBGM = true;
                    }
                }

                return isNecessaryForPlayBGM;

            }

            void StopUnsameNameBGM(Transform target)
            {
                ClipRequestReturnToPool(target);
            }
        }

        internal void RequestPlayBGM(AudioStorage storage, float playStartTime = 0f, bool isLoop = true)
        {

            // 현재 재생중인 BGM이 있는지 확인한다.
            RequestStopTheRestBGM(storage, out var isneedPlay);

            if (isneedPlay == false)
                return;

            var basicSoundClip = GetBGMFromPool();
            basicSoundClip.SetInitialization(storage, playStartTime, isLoop, storage.Volume * _bgmVolume);

        }

        #endregion

        #region Request Ambience

        internal BasicSoundClipPlay_Common RequestPlayAmbience(AudioStorage storage, Vector3 pos, bool isLoop = true, Transform setFollowTarget = null)
        {
            // Set Basic object pooling setting.
            var tSoundClipCommon = GetAmbienceFromPool();

            tSoundClipCommon.SetInitialization
                (storage,
                storage.PlayStartTime,
                isLoop,
                storage.Volume * _ambientVolume,
                useAudioSourceSetting: true,
                setFollowTarget
                );

            tSoundClipCommon.transform.position = pos;

            return tSoundClipCommon;
        }

        #endregion

        #region Request Clip

        internal BasicSoundClipPlay_Common RequestPlayClip(AudioStorage storage, float playStartTime = 0f, bool isLoop = false, bool useStorageStartTime = false, Transform setFollowTarget = null)
        {
            var t_soundClipCommon = GetClipFromPool();

            InitializePlayeClip(playStartTime, isLoop, useStorageStartTime, setFollowTarget, storage, t_soundClipCommon);

            t_soundClipCommon.transform.position = Vector3.zero;

            return t_soundClipCommon;
        }

        internal BasicSoundClipPlay_Common RequestPlayClip(AudioStorage storage, Vector3 pos, float playStartTime = 0f, bool isLoop = false, bool useStorageStartTime = false, Transform setFollowTarget = null)
        {
            var t_soundClipCommon = GetClipFromPool();

            InitializePlayeClip(playStartTime, isLoop, useStorageStartTime, setFollowTarget, storage, t_soundClipCommon);

            t_soundClipCommon.transform.position = pos;

            return t_soundClipCommon;
        }

        internal BasicSoundClipPlay_Common[] RequestPlayClip(AudioStorage[] storages, float playStartTime = 0f, bool isLoop = false, bool useStorageStartTime = false, Transform setFollowTarget = null)
        {
            List<BasicSoundClipPlay_Common> _soundList = new List<BasicSoundClipPlay_Common>();

            foreach (var sound in storages)
            {
                var t_soundClipCommon = GetClipFromPool();

                InitializePlayeClip(playStartTime, isLoop, useStorageStartTime, setFollowTarget, sound, t_soundClipCommon);

                t_soundClipCommon.transform.position = Vector3.zero;

                _soundList.Add(t_soundClipCommon);
            }

            return _soundList.ToArray();
        }

        internal BasicSoundClipPlay_Common[] RequestPlayClip(AudioStorage[] storages, Vector3 pos, float playStartTime = 0f, bool isLoop = false, bool useStorageStartTime = false, Transform setFollowTarget = null)
        {
            List<BasicSoundClipPlay_Common> _soundList = new List<BasicSoundClipPlay_Common>();

            foreach (var sound in storages)
            {
                var t_soundClipCommon = GetClipFromPool();

                InitializePlayeClip(playStartTime, isLoop, useStorageStartTime, setFollowTarget, sound, t_soundClipCommon);

                t_soundClipCommon.transform.position = pos;

                _soundList.Add(t_soundClipCommon);
            }

            return _soundList.ToArray();
        }

        private void InitializePlayeClip(float playStartTime, bool isLoop, bool useStorageStartTime, Transform setFollowTarget, AudioStorage sound, BasicSoundClipPlay_Common t_soundClipCommon)
        {
            if (setFollowTarget == null)
                SetSoundAttributions(sound, playStartTime, isLoop, useStorageStartTime, t_soundClipCommon);
            else
                SetSoundAttributions(sound, playStartTime, isLoop, useStorageStartTime, t_soundClipCommon, setFollowTarget);
        }

        private void SetSoundAttributions(AudioStorage storage, float playStartTime, bool isLoop, bool useStorageStartTime, BasicSoundClipPlay_Common t_soundClipCommon)
        {
            // Set sound Attributions.
            if (useStorageStartTime == true)
            {
                t_soundClipCommon.SetInitialization(storage, storage.PlayStartTime, isLoop, storage.Volume * _soundClipVolume);
            }
            else
            {
                t_soundClipCommon.SetInitialization(storage, playStartTime, isLoop, storage.Volume * _soundClipVolume);
            }
        }

        private void SetSoundAttributions(AudioStorage storage, float playStartTime, bool isLoop, bool useStorageStartTime, BasicSoundClipPlay_Common t_soundClipCommon, Transform followObject)
        {
            // Set sound Attributions.
            if (useStorageStartTime == true)
            {
                t_soundClipCommon.SetInitialization(storage, storage.PlayStartTime, isLoop, storage.Volume * _soundClipVolume, _isFollowObject: followObject);
            }
            else
            {
                t_soundClipCommon.SetInitialization(storage, playStartTime, isLoop, storage.Volume * _soundClipVolume, _isFollowObject: followObject);
            }
        }

        #endregion

        #region GetClipsFromPool

        internal BasicSoundClipPlay_Common GetBGMFromPool()
        {
            _currentBGMNumber++;
            if (_currentBGMNumber >= _numberOfBGMSOundObjects)
                _currentBGMNumber = 0;
            _BGMSoundObjectsPool[_currentBGMNumber].SetActive(true);
            return _BGMSoundObjectsPool[_currentBGMNumber].GetComponent<BasicSoundClipPlay_Common>();
        }

        internal BasicSoundClipPlay_Common GetAmbienceFromPool()
        {
            _currentAmbienceNumber++;
            if (_currentAmbienceNumber >= _numberOfAmbienceSoundObjects)
                _currentAmbienceNumber = 0;
            _ambienceSoundObjectsPool[_currentAmbienceNumber].SetActive(true);
            return _ambienceSoundObjectsPool[_currentAmbienceNumber].GetComponent<BasicSoundClipPlay_Common>();
        }

        internal BasicSoundClipPlay_Common GetClipFromPool()
        {
            _currentClipNumber++;
            if (_currentClipNumber >= _numberOfClipSoundObjects)
                _currentClipNumber = 0;
            _clipSoundObjectsPool[_currentClipNumber].SetActive(true);
            return _clipSoundObjectsPool[_currentClipNumber].GetComponent<BasicSoundClipPlay_Common>();
        }

        #endregion

        #region Methuds

        public void ClipRequestReturnToPool(Transform thereSelves)
        {
            thereSelves.position = Vector3.zero;

            // Set parent to pool transform.
            switch (thereSelves.gameObject.GetComponent<BasicSoundClipPlay_Common>().GetSoundPoolTag())
            {
                case SoundPoolTag.Clip:
                    thereSelves.parent = _clipPool.transform;
                    break;
                case SoundPoolTag.Ambience:
                    thereSelves.parent = _ambiencePool.transform;
                    break;
                case SoundPoolTag.BGM:
                    thereSelves.name = "This set for bgm not work";
                    thereSelves.parent = _BGMPool.transform;
                    break;
            }

            thereSelves.gameObject.SetActive(false);
        }

        #endregion

    }

}
