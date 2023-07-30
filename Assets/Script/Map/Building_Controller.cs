using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorningBird;
using Sirenix.OdinInspector;
using System.Linq;
using TMPro;

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

        [SerializeField] Dictionary<Building_FlowerPoint, EBuildingProtesterState> _zoneOneFlowerStatePair = new Dictionary<Building_FlowerPoint, EBuildingProtesterState>(200);
        [SerializeField] Dictionary<Building_FlowerPoint, EBuildingProtesterState> _zoneTwoFlowerStatePair = new Dictionary<Building_FlowerPoint, EBuildingProtesterState>(200);
        [SerializeField] Dictionary<Building_FlowerPoint, EBuildingProtesterState> _zoneThreeFlowerStatePair = new Dictionary<Building_FlowerPoint, EBuildingProtesterState>(200);
        [SerializeField] Dictionary<Building_FlowerPoint, EBuildingProtesterState> _zoneFourFlowerStatePair = new Dictionary<Building_FlowerPoint, EBuildingProtesterState>(200);
        //[SerializeField] Dictionary<Building_FlowerPoint, EBuildingProtesterState> _zoneFiveFlowerStatePair = new Dictionary<Building_FlowerPoint, EBuildingProtesterState>(200);

        [SerializeField] GameObject _mainMenuSceneLoader;
        [SerializeField] GameObject _mainMenuSceneLoader2;
        [SerializeField] GameObject _mainMenuSceneLoader3;
        [SerializeField] GameObject _mainMenuSceneLoader4;

        [SerializeField] Building_FlowerPoint _dumpFlowerPoint = new Building_FlowerPoint();
        public Building_FlowerPoint DumpFlowerPoint => _dumpFlowerPoint;

        [FoldoutGroup("CheckingZone")]
        [SerializeField] float _checkingTimeForZoneComplite = 2f;
        [SerializeField] float _innerCheckingTimeForZoneComplite = 0f;
        [SerializeField] bool[] _zoneComplitingState = new bool[4];

        [SerializeField] bool _isGameDone;

        public Dictionary<Building_FlowerPoint, EBuildingProtesterState> GetZoneFlowerStatePair(int zoneNumber)
        {
            switch (zoneNumber)
            {
                default:
                    Debug.Assert(false);
                    return _zoneOneFlowerStatePair;
                case 0: return _zoneOneFlowerStatePair;
                case 1: return _zoneTwoFlowerStatePair;
                case 2: return _zoneThreeFlowerStatePair;
                case 3: return _zoneFourFlowerStatePair;
                //case 4: return _zoneFiveFlowerStatePair;
            }
        }

        public void SetZoneFlowerStatePair(int zoneNumber, Building_FlowerPoint flowerPoint, EBuildingProtesterState buildingState)
        {
            var t_dic = GetZoneFlowerStatePair(zoneNumber);
            t_dic[flowerPoint] = buildingState;
        }

        public void RegistZoneFlowerStatePair(int zoneNumber, Building_FlowerPoint flowerPoint, EBuildingProtesterState buildingState)
        {
            switch (zoneNumber)
            {
                default:
                    Debug.Assert(false);
                    _zoneOneFlowerStatePair.Add(flowerPoint, buildingState);
                    break;
                case 0: _zoneOneFlowerStatePair.Add(flowerPoint, buildingState); 
                    break;
                case 1: _zoneTwoFlowerStatePair.Add(flowerPoint, buildingState);
                    break;
                case 2: _zoneThreeFlowerStatePair.Add(flowerPoint, buildingState);
                    break;
                case 3: _zoneFourFlowerStatePair.Add(flowerPoint, buildingState);
                    break;
                //case 4: _zoneFiveFlowerStatePair.Add(flowerPoint, buildingState);
                  //  break;
            }
        }

        public void DeleteZoneFlowerStatePair(int zoneNumber, Building_FlowerPoint flowerPoint)
        {
            switch (zoneNumber)
            {
                default:
                    Debug.Assert(false);
                    _zoneOneFlowerStatePair.Remove(flowerPoint);
                    break;
                case 0:
                    _zoneOneFlowerStatePair.Remove(flowerPoint);
                    break;
                case 1:
                    _zoneTwoFlowerStatePair.Remove(flowerPoint);
                    break;
                case 2:
                    _zoneThreeFlowerStatePair.Remove(flowerPoint);
                    break;
                case 3:
                    _zoneFourFlowerStatePair.Remove(flowerPoint);
                    break;
                //case 4:
                  //  _zoneFiveFlowerStatePair.Remove(flowerPoint);
                  //  break;
            }
        }

        public void GetZoneFlowerStatePair(int zoneNumber, out Building_FlowerPoint[] flowerPoint, out EBuildingProtesterState[] buildingState)
        {

            var t_dic = GetZoneFlowerStatePair(zoneNumber);
            flowerPoint = t_dic.Keys.ToArray();
            buildingState = t_dic.Values.ToArray();

        }

        public bool IsZoneComplite(int zoneNumber)
        {
            return _zoneComplitingState[zoneNumber];
        }

        public void SetZoneCompolite(int zoneNumber, bool state)
        {
            _zoneComplitingState[zoneNumber] = state;
        }

        public bool IsAllZoneComplite()
        {
            for (int i = 0; i < 4; i++)
            {
                if (IsZoneComplite(i) == false)
                {
                    return false;
                }
            }
            return true;
        }


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

        [SerializeField] bool _endIsCalled = false;

        private void Update()
        {
            _isGameDone = IsAllZoneComplite();

            if (_isGameDone == true && _endIsCalled == false)
            {
                Camera.main.GetComponent<CameraFollow>().ToggleEndGame();
                StartCoroutine(BackToMainMenuIE());
                _endIsCalled = true;
            }

            bool CheckInnerTimeForZoneComplite()
            {
                if (_innerCheckingTimeForZoneComplite > 0)
                {
                    _innerCheckingTimeForZoneComplite -= Time.deltaTime;
                    return false;
                }

                _innerCheckingTimeForZoneComplite = _checkingTimeForZoneComplite;
                return true;
            }
            if(CheckInnerTimeForZoneComplite() == false)
            {
                return;
            }

            void CheckZoneIsComplite()
            {
                for (int i = 0; i < 4; i++)
                {
                    var zoneFlowerStatePair = GetZoneFlowerStatePair(i);
                    if(zoneFlowerStatePair == null || zoneFlowerStatePair.Count == 0)
                    {
                        SetZoneCompolite(i, true);
                        break;
                    }

                    bool condition = true;

                    foreach (KeyValuePair<Building_FlowerPoint, EBuildingProtesterState> flowerStatePair in zoneFlowerStatePair)
                    {
                        if (flowerStatePair.Value == EBuildingProtesterState.None || flowerStatePair.Value == EBuildingProtesterState.Protest)
                        {
                            condition = false;
                            break;
                        }
                    }

                    SetZoneCompolite(i, condition);
                }

            }
            CheckZoneIsComplite();

        }

        IEnumerator BackToMainMenuIE()
        {
            yield return new WaitForSeconds(10f);
            Instantiate(_mainMenuSceneLoader);
        }


    }

}