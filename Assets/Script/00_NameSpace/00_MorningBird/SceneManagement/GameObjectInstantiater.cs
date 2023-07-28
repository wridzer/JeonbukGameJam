using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MorningBird.SceneManagement
{
    public class GameObjectInstantiater : MonoBehaviour
    {
        public void InsatantiateGameObject(GameObject go)
        {
            Instantiate(go);
        }
    }

}

