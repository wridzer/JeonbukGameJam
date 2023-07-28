using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MorningBird.Sound
{ 
    public class BGMRequestor : MonoBehaviour
    {
        [SerializeField] AudioStorage[] _BGMs;
        [SerializeField] float _waitTimeBeforeFirstStart= 2.0f;
        [SerializeField] float _BGMPlayFirstStartTime = 0.0f;
        [SerializeField] bool _turnOffMusicWhenItDisabled = false;
        [SerializeField] bool _requestTurnOffBGM = false;

        [SerializeField] float _waitTimeForNextBGM;
        [SerializeField] bool _isSongEnded = false;
        [SerializeField] int _currentSongNumber = 0;

        [SerializeField] TextMeshProUGUI _titleSong;

        void OnEnable()
        {

            if(_requestTurnOffBGM == true)
            {
                SoundManager.Instance.RequestStopAllBGM();
                this.enabled = false;
            }
            else
            {
                StartCoroutine(PlayBGMLatly());
                _currentSongNumber = 0;
            }
        }

    #if UNITY_EDITOR

        [SerializeField] bool _test;

    #endif

        private void Update()
        {
            if(_isSongEnded == false)
            {
                _waitTimeForNextBGM -= Time.deltaTime;
            }
            else // _isSongEnded == true
            {
                PlayNextSong();
            }

            if (_waitTimeForNextBGM <= 0f)
            {
                _isSongEnded = true;
            }

    #if UNITY_EDITOR

            if(_test)
            {
                _test = false;
                PlayNextSong();
            }

    #endif

        }

        private void PlayNextSong()
        {
            _currentSongNumber++;
            if (_currentSongNumber >= _BGMs.Length)
            {
                _currentSongNumber = 0;
            }

            SoundManager.Instance.RequestPlayBGM(_BGMs[_currentSongNumber], isLoop:false);
            _waitTimeForNextBGM = _BGMs[_currentSongNumber].audioClip.length;
            _isSongEnded = false;

            if(_titleSong != null)
            {
                _titleSong.text = _BGMs[_currentSongNumber].Name;
            }
        }

        IEnumerator PlayBGMLatly()
        {
            _waitTimeForNextBGM = _waitTimeBeforeFirstStart + 2f;
            _isSongEnded = false;
            yield return new WaitForSeconds(_waitTimeBeforeFirstStart);
            SoundManager.Instance.RequestPlayBGM(_BGMs[0], _BGMPlayFirstStartTime, isLoop: false);
            _waitTimeForNextBGM = _BGMs[0].audioClip.length;

            if (_titleSong != null)
            {
                _titleSong.text = _BGMs[0].Name;
            }
        }

        private void OnDisable()
        {
            if (_turnOffMusicWhenItDisabled == true)
                SoundManager.Instance.RequestStopAllBGM();
        }
    }

}


