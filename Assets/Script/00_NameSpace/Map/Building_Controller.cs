using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorningBird;
using Sirenix.OdinInspector;
using System.Linq;

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

        public Building_Common[] GetStateBuildings(EBuildingProtesterState state)
        {
            List<Building_Common> t_buildlings = new List<Building_Common>(1000);
            Building_Common[] t_keys = _buildingStatePair.Keys.ToArray();

            switch (state)
            {
                case EBuildingProtesterState.None:
                    Find(state);
                    break;
                case EBuildingProtesterState.Flower:
                    Find(state);
                    break;
                case EBuildingProtesterState.Protest:
                    Find(state);
                    break;
                default:
                    Find(state);
                    break;
            }

            return t_buildlings.ToArray();

            void Find(EBuildingProtesterState state)
            {
                for (int i = 0; i < _buildingStatePair.Count; i++)
                {
                    if (_buildingStatePair[t_keys[i]] == state)
                    {
                        t_buildlings.Add(t_keys[i]);
                    }
                }
            }
        }


        public void SetBuildingState(Building_Common buildingCommon, EBuildingProtesterState state)
        {
            if(_buildingStatePair.ContainsKey(buildingCommon))
            {
                _buildingStatePair[buildingCommon] = state;
            }
            else
            {
                AddBuildingInList(buildingCommon, state);
            }
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