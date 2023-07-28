using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MorningBird.Sound
{
    public class AmbienceSoundCreator : MonoBehaviour
    {
        [SerializeField] AudioStorage _ambienceSound;
        [SerializeField] BasicSoundClipPlay_Common _soundCommon;

        SoundManager _SM;
        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.tag == "Player")
            {
                if (_SM == null)
                    _SM = SoundManager.Instance;

                _soundCommon = _SM.RequestPlayAmbience(_ambienceSound, this.transform.position, isLoop: true);

            }    
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.transform.tag == "Player")
            {
                _soundCommon.ReturnToPool();
                _soundCommon = null;
            }
        }

        private void OnDisable()
        {
            if (_SM != null && _soundCommon != null)
            {
                _soundCommon.ReturnToPool();
                _soundCommon = null;
            }
            else
            {
                // stop sound..
            }
        }
    }

}
