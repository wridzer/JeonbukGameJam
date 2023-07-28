using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MorningBird.Sound
{
    public class SimpleClipSoundCreator : MonoBehaviour
    {
        [SerializeField] AudioStorage[] _audios;

        [SerializeField] bool _test;

        // Update is called once per frame
        void Update()
        {
            if(_test == true)
            {
                if(_audios != null)
                {
                    for (int i = 0; i < _audios.Length; i++)
                    {
                        SoundManager.Instance.RequestPlayClip(_audios[i], this.transform.position, _audios[i].PlayStartTime);
                    }
                }

                _test = false;
            }
        }
    }

}
