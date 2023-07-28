using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorningBird;
using Sirenix.OdinInspector;

namespace Game.Building
{
    public class Building_Controller : SerializedMonoBehaviour
    {

        private static Building_Controller instance;

        public static Building_Controller Instance { get { return instance; } }

        private void Awake()
        {
            instance = this;
        }

        [TitleGroup("Debug")]
        [SerializeField] Dictionary<Building_Common, EBuildingProtesterState> _buildingStatePair = new Dictionary<Building_Common, EBuildingProtesterState>(1000);

        public Dictionary<Building_Common, EBuildingProtesterState> BuildingCommons => _buildingStatePair;

        public void SetBuildingState(Building_Common buildingCommon, EBuildingProtesterState state)
        {

        }

        public void AddBuildingInList(Building_Common buildingCommon, EBuildingProtesterState state)
        {
            _buildingStatePair.Add(buildingCommon, state);
        }

        public void DeleteBuildingInList(Building_Common buildingCommon)
        {
            _buildingStatePair.Remove(buildingCommon);
        }


    }

}