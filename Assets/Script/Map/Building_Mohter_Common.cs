using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MorningBird;

namespace Game.Building
{
    public enum EBuildingProtesterState
    {
        None,
        Flower,
        Protest
    }

    public class Building_Mohter_Common : SerializedMonoBehaviour
    {
        public EBuildingProtesterState BuildingState => _state;

        [TitleGroup("PreDefine")]
        [SerializeField] protected EBuildingProtesterState _state;

        //[TitleGroup("Debug")]


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void UpdateBuildingMaterial(EBuildingProtesterState state)
        {

        }


    } 
}
