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
        [SerializeField] Dictionary<Building_FlowerPoint, EBuildingProtesterState> _flowerStatePair = new Dictionary<Building_FlowerPoint, EBuildingProtesterState>(500);

        public Dictionary<Building_FlowerPoint, EBuildingProtesterState> FlowerPointPairs => _flowerStatePair;

        public Building_FlowerPoint[] GetStateBuildings(EBuildingProtesterState state)
        {
            List<Building_FlowerPoint> t_buildlings = new List<Building_FlowerPoint>(500);
            Building_FlowerPoint[] t_keys = _flowerStatePair.Keys.ToArray();

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
                for (int i = 0; i < _flowerStatePair.Count; i++)
                {
                    if (_flowerStatePair[t_keys[i]] == state)
                    {
                        t_buildlings.Add(t_keys[i]);
                    }
                }
            }
        }

        public Building_FlowerPoint[] GetFlowerPointForProtester()
        {
            List<Building_FlowerPoint> t_buildlings = new List<Building_FlowerPoint>(500);
            Building_FlowerPoint[] t_keys = _flowerStatePair.Keys.ToArray();

            Find(EBuildingProtesterState.None);
            Find(EBuildingProtesterState.Flower);

            return t_buildlings.ToArray();

            void Find(EBuildingProtesterState state)
            {
                for (int i = 0; i < _flowerStatePair.Count; i++)
                {
                    if (_flowerStatePair[t_keys[i]] == state)
                    {
                        t_buildlings.Add(t_keys[i]);
                    }
                }
            }
        }


        public void SetBuildingState(Building_FlowerPoint flowerPoint, EBuildingProtesterState state)
        {
            if(_flowerStatePair.ContainsKey(flowerPoint))
            {
                _flowerStatePair[flowerPoint] = state;
            }
            else
            {
                AddBuildingInList(flowerPoint, state);
            }
        }

        public void AddBuildingInList(Building_FlowerPoint flowerPoint, EBuildingProtesterState state)
        {
            _flowerStatePair.Add(flowerPoint, state);
        }

        public void DeleteBuildingInList(Building_FlowerPoint flowerPoint)
        {
            _flowerStatePair.Remove(flowerPoint);
        }


    }

}