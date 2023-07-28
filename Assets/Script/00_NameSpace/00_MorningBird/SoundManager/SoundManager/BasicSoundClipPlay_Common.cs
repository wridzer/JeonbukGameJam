using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MorningBird.Sound
{
    public class BasicSoundClipPlay_Common : MonoBehaviour
    {
        [SerializeField] Transform _followObjectTransform;

        [SerializeField] SoundPoolTag _soundPoolTag;

        [SerializeField] float _clipDuration;
        [SerializeField] float _remainTime;
        [SerializeField] float _delayTime;

        [SerializeField] AudioStorage _currentAudioStorage;
        internal AudioStorage GetAudioStorage => _currentAudioStorage;
        [SerializeField] AudioSource _basicAudioSource;

        [SerializeField] bool _isEndedMusicDelay = false;
        bool _isSoundFollowingObject;

        void Initialization()
        {
            _followObjectTransform = null;

            _clipDuration = _currentAudioStorage.audioClip.length;
            _remainTime = _delayTime + _clipDuration;
            _delayTime = 0f;

            _isEndedMusicDelay = false;

            this.name = _currentAudioStorage.Name;
            _basicAudioSource = this.transform.GetComponent<AudioSource>();
            _basicAudioSource.clip = _currentAudioStorage.audioClip;
        }

        private void Start()
        {
            _basicAudioSource = this.transform.GetComponent<AudioSource>();
            _basicAudioSource = this.transform.GetComponent<AudioSource>();
        }

        protected virtual void OnDisable()
        {
            ReturnToPool();
        }

        protected virtual void Update()
        {
            DecreaseRemainTime();
            CheckReturnToPool();
            FollowingObject();
        }

        protected virtual void DecreaseRemainTime()
        {
            _remainTime -= Time.deltaTime;
        }
        protected virtual void CheckReturnToPool()
        {
            if (_remainTime < 0f)
            {
                ReturnToPool();
            }
        }
        protected void FollowingObject()
        {
            if (_isSoundFollowingObject == true)
            {
                if (_followObjectTransform == null)
                {
                    _isSoundFollowingObject = false;
                    return;
                }

                this.transform.position = _followObjectTransform.position;
                this.transform.rotation = _followObjectTransform.rotation;

            }
        }


        private void FixedUpdate()
        {
            if (_isEndedMusicDelay == false)
                CalcualtePlayDelay();
        }

        private void CalcualtePlayDelay()
        {
            if (_delayTime > 0f)
            {
                _delayTime -= Time.fixedDeltaTime;
                _isEndedMusicDelay = false;
            }
            else
            {
                _basicAudioSource.Play();
                _isEndedMusicDelay = true;
            }
        }

        internal void ReturnToPool()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.ClipRequestReturnToPool(this.transform);
        }

        internal void SetSoundPoolTag(SoundPoolTag poolTag) => _soundPoolTag = poolTag;
        internal SoundPoolTag GetSoundPoolTag() => _soundPoolTag;

        internal void SetDelayTime(float value) => _delayTime = value;

        internal void SetAudioVolume(float value) => _basicAudioSource.volume = value;

        #region SetInitializations

        internal void SetInitialization(AudioStorage audioStorage, float volume, bool useAudioSourceSetting = true, Transform _isFollowObject = null)
        {
            _currentAudioStorage = audioStorage;
            Initialization();

            if (_isFollowObject != null)
            {
                SetFollowingObject(_isFollowObject);
            }

            if (useAudioSourceSetting == true)
            {
                SetAudioSourceSettingWithAudioStorage(audioStorage, volume);
            }

            _remainTime = _delayTime + _clipDuration;
        }

        internal void SetInitialization(AudioStorage audioStorage, float playStartTime, float volume, bool useAudioSourceSetting = true, Transform _isFollowObject = null)
        {
            _currentAudioStorage = audioStorage;
            Initialization();
            if (_isFollowObject != null)
            {
                SetFollowingObject(_isFollowObject);
            }

            if (useAudioSourceSetting == true)
            {
                SetAudioSourceSettingWithAudioStorage(audioStorage, volume);
            }

            if (playStartTime < 0f)
            {
                _delayTime += Mathf.Abs(playStartTime);
                _remainTime = _delayTime + _clipDuration;
                _isEndedMusicDelay = false;
            }
            else
            {
                _basicAudioSource.time = playStartTime;
            }

        }


        internal void SetInitialization(AudioStorage audioStorage, float playStartTime, bool isLoop, float volume, bool useAudioSourceSetting = true, Transform _isFollowObject = null)
        {
            _currentAudioStorage = audioStorage;
            Initialization();
            if (_isFollowObject != null)
            {
                SetFollowingObject(_isFollowObject);
            }


            if (useAudioSourceSetting == true)
            {
                SetAudioSourceSettingWithAudioStorage(audioStorage, volume);
            }

            if(playStartTime < 0f)
            {
                _delayTime = Mathf.Abs(playStartTime);
                _remainTime = _delayTime + _clipDuration;
                _isEndedMusicDelay = false;
            }
            else
            {
                _basicAudioSource.time = playStartTime;
            }


            if (isLoop == true)
            {
                LoopingSetting(isLoop);
            }

        }


        #endregion

        #region Mathods

        private void SetAudioSourceSettingWithAudioStorage(AudioStorage audioStorage, float volume)
        {
            _basicAudioSource.bypassEffects = audioStorage.BypassEffects;
            _basicAudioSource.bypassListenerEffects = audioStorage.BypassListenerEffects;
            _basicAudioSource.bypassReverbZones = audioStorage.BypassReverbZones;

            _basicAudioSource.priority = audioStorage.Priority;
            _basicAudioSource.volume = volume;
            _basicAudioSource.pitch = audioStorage.Pitch;
            _basicAudioSource.panStereo = audioStorage.StereoPan;
            _basicAudioSource.spatialBlend = audioStorage.SpatialBlend;
            _basicAudioSource.reverbZoneMix = audioStorage.ReverbZoneMix;
            _basicAudioSource.dopplerLevel = audioStorage.DopplerLevel;
            _basicAudioSource.spread = audioStorage.Spread;
            _basicAudioSource.rolloffMode = audioStorage.RolloffMode;
            _basicAudioSource.minDistance = audioStorage.MinDistance;
            _basicAudioSource.maxDistance = audioStorage.MaxDistance;
        }

        private void LoopingSetting(bool isLoop)
        {
            _basicAudioSource.loop = isLoop;
            _clipDuration = _currentAudioStorage.audioClip.length * 100;
            _remainTime = _currentAudioStorage.audioClip.length * 100;
        }

        private void SetFollowingObject(Transform transform)
        {
            _isSoundFollowingObject = true;
            _followObjectTransform = transform;
        }

        #endregion
    }

}
