using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace MorningBird.Sound
{
    [CreateAssetMenu(fileName = "New AudioClipStorage", menuName = "AuidoClipStorage")]
    public sealed class AudioStorage : SerializedScriptableObject
    {
#if UNITY_EDITOR

        private void OnValidate()
        {
            ChangeName();
        }

        void ChangeName()
        {
            _name = this.name;
        }

        [SerializeField] string _name;
#endif

        public string Name => name;

        [SerializeField] AudioClip _audioClip;
        public AudioClip audioClip => _audioClip;

        [SerializeField] bool _bypassEffects = false;
        [SerializeField] bool _bypassListenerEffects = false;
        [SerializeField] bool _bypassReverbZones = false;

        public bool BypassEffects => _bypassEffects;
        public bool BypassListenerEffects => _bypassListenerEffects;
        public bool BypassReverbZones => _bypassReverbZones;

        [SerializeField] float _playStartTime = 0f;
        public float PlayStartTime => _playStartTime;

        [Range(0, 255)]
        [SerializeField] int _priority = 128;
        public int Priority => _priority;

        [Range(0, 1.2f)]
        [SerializeField] float _volume = 0.75f;
        public float Volume => _volume;

        [Range(-3f, 3f)]
        [SerializeField] float _pitch = 1f;
        public float Pitch => _pitch;

        [Range(-1f, 1f)]
        [SerializeField] float _stereoPan = 0;
        public float StereoPan => _stereoPan;

        [Range(0, 1f), Tooltip("0 is 2D, 1 is 3D.")]
        [SerializeField] float _spatialBlend = 1f;
        public float SpatialBlend => _spatialBlend;

        [Range(0, 1.1f)]
        [SerializeField] float _reverbZoneMix = 1f;
        public float ReverbZoneMix => _reverbZoneMix;

        [FoldoutGroup("3D Sound Settings")]
        [SerializeField, Range(0f, 5f)] float _dopplerLevel = 0f;

        [FoldoutGroup("3D Sound Settings")]
        [SerializeField, Range(0, 360)] int _spread = 0;

        [FoldoutGroup("3D Sound Settings")]
        [SerializeField] AudioRolloffMode _volumeRolloff = AudioRolloffMode.Logarithmic;

        [FoldoutGroup("3D Sound Settings")]
        [SerializeField] float _minDistance = 1f;

        [FoldoutGroup("3D Sound Settings")]
        [SerializeField] float _maxDistance = 25f;

        public float DopplerLevel => _dopplerLevel;
        public int Spread => _spread;
        public AudioRolloffMode RolloffMode => _volumeRolloff;
        public float MinDistance => _minDistance;
        public float MaxDistance => _maxDistance;


    }

}

