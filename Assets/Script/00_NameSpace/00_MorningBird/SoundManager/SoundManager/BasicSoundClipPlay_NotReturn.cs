using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MorningBird.Sound
{
    public class BasicSoundClipPlay_NotReturn : BasicSoundClipPlay_Common
    {

        protected override void Update()
        {
            DecreaseRemainTime();
            FollowingObject();
        }

        protected override void CheckReturnToPool()
        {

        }

        protected override void OnDisable()
        {
            
        }
    }
}

