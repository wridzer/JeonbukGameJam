using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MorningBird;
using UnityEngine.AI;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using Game.Building;
using System;

namespace Game.NPC
{
    public enum ECivilionState
    {
        Peace,
        Protester
    }

    public class Civilion_Common : SerializedMonoBehaviour
    {
        [TitleGroup("PreDefine")]
        [SerializeField] NavMeshAgent _navMeshAgent;
        [SerializeField] float _buildingProtestingTime = 5f;
        [SerializeField] float _buildingProtestingAfterWaitTime = 5f;
        [SerializeField] Vector3 _initializePosition = Vector3.zero;


        [TitleGroup("Debug")]
        [SerializeField] Game.Building.Building_Controller _buildingCon;
        [SerializeField] Building_FlowerPoint _selectedFlowerPoint;
        [SerializeField] float _innerbuildingProtestingTime;
        [SerializeField] ECivilionState _civilionState = ECivilionState.Protester;

        [SerializeField] bool _isNPCReachToBuilding = false;
        [SerializeField] float _destinationDistance;
        [SerializeField] float _navMeshMagnitude;
        [SerializeField] float _transformMagnitude;

        [SerializeField] EZoneNumber _zoneNumber;

        [SerializeField] bool _isSettedEndDestination;

        [SerializeField] bool _isCalledIEA;

        [SerializeField] ToggleProtestor[] _toggleProtestors;



        void Start()
        {
            if(_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            _buildingCon = Game.Building.Building_Controller.Instance;

            StartCoroutine(IEWaitSetDestination(3f));

            _navMeshAgent.avoidancePriority = Random.Range(0, 100);

            _toggleProtestors = this.GetComponentsInChildren<ToggleProtestor>();
        }

        IEnumerator IEWaitSetDestination(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SetDestination();
            _isCalledIEA = false;
        }

        void Update()
        {

            if(_isSettedEndDestination == false)
            {
                if(CheckReachToBuilding() == true && CheckTimeIsDone() == true)
                {
                    if(_isCalledIEA == false)
                    {
                        DoProtestingWork();

                    }
                }
            }
            else // _isSettedEndDestination == true
            {
                if (CheckReachToBuilding() == true && CheckTimeIsDone() == true)
                {
                    Destroy(this.gameObject);
                }
            }

        }


        private void OnTriggerStay(Collider other)
        {

            FindBuildingAndCheckSame(other);

            void FindBuildingAndCheckSame(Collider other)
            {
                if (other.transform.TryGetComponent<Building_FlowerPoint>(out Building_FlowerPoint flowerPoint) == true)
                {
                    if (flowerPoint == _selectedFlowerPoint)
                    {
                        return;
                    }

                    _selectedFlowerPoint = flowerPoint;
                    _innerbuildingProtestingTime = _buildingProtestingTime;
                }
            }
        }

        public void SetStateOfProtester(ECivilionState state)
        {
            switch (state)
            {
                case ECivilionState.Peace:
                    FindHome();
                    break;
                case ECivilionState.Protester:
                    SetDestination();
                    break;
                default:
                    FindHome();
                    break;
            }
        }

        private bool CheckReachToBuilding()
        {
            var t_vector3 = (_navMeshAgent.destination - transform.position);

            _destinationDistance = t_vector3.sqrMagnitude;
            

            if (_destinationDistance > 15f)
            {
                return false;
            }
            return true;


        }

        private bool CheckTimeIsDone()
        {
            if(_isSettedEndDestination == false)
            {
                if (_selectedFlowerPoint == null)
                    return false;
            }


            if (_innerbuildingProtestingTime > 0)
            {
                _innerbuildingProtestingTime -= Time.deltaTime;
                return false;
            }

            return true;
        }

        private void DoProtestingWork()
        {
            switch (_civilionState)
            {
                case ECivilionState.Peace:
                    SetStateOfProtester(ECivilionState.Protester);
                    SetDestination();
                    break;
                case ECivilionState.Protester:
                    ProtestToFlowerPooint();
                    break;
                default:
                    break;
            }
        }

        private void ProtestToFlowerPooint()
        {
            Collider[] collliders = Physics.OverlapSphere(transform.position, 5f);
            Building_FlowerPoint t_flowerPoint;

            for (int i = 0; i < collliders.Length; i++)
            {
                if(collliders[i].TryGetComponent<Building_FlowerPoint>(out t_flowerPoint) == true)
                {
                    t_flowerPoint.SetFlowerPointCondition(EBuildingProtesterState.Protest);
                    break;
                }
            }

            if(_innerbuildingProtestingTime > 1f)
            {
                Debug.LogError("Time for building is exceed");
            }

            if(_isCalledIEA == false)
            {
                StartCoroutine(IEWaitSetDestination(_buildingProtestingAfterWaitTime));
                _isCalledIEA = true;

            }


        }

        [Button]
        private void FindHome()
        {
            SetDestinationToEnd();
            SetProtestersCondition(ECivilionState.Peace);

        }

        [Button]
        private void SetDestination()
        {
            // 0. Initialize
            {
                StopCoroutine(IECheckIsZoneComplite());
            }

            // is all zone Cleared
            {
                if(_buildingCon.IsAllZoneComplite() == true)
                { 
                    SetDestinationToEnd();
                    return;
                }
            }

            _zoneNumber = EZoneNumber.one;

            // is selected zone is complited
            {
                bool find = false;
                // Random Selection
                for (int i = 0; i < 20; i++)
                {
                    _zoneNumber = (EZoneNumber)Random.Range(0, 3);

                    if (_buildingCon.IsZoneComplite((int)_zoneNumber) == false)
                    {
                        find = true;
                        break;
                    }
                }

                // Pick Selection
                if(find == false)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (_buildingCon.IsZoneComplite(i) == false)
                        {
                            _zoneNumber = (EZoneNumber)i;
                            break;
                        }
                    }
                }
            }

            Building_FlowerPoint flowerPoint = _buildingCon.DumpFlowerPoint;

            // Set Destination
            {
                // if zone don't have any flower point
                if (_buildingCon.GetZoneFlowerStatePair((int)_zoneNumber).Count == 0)
                {
                    SetDestination();
                }

                _buildingCon.GetZoneFlowerStatePair((int)_zoneNumber, out var flowerPoints, out var buildingState);

                // find CaptureAble Building
                {
                    // Find with Random
                    for (int i = 0; i < buildingState.Length; i++)
                    {
                        int random = Random.Range(0, buildingState.Length);

                        if (buildingState[random] == EBuildingProtesterState.None || buildingState[random] == EBuildingProtesterState.Flower)
                        {
                            flowerPoint = flowerPoints[random];
                            break;
                        }

                    }

                    // Must Find
                    if(flowerPoint == _buildingCon.DumpFlowerPoint)
                    {
                        for (int i = 0; i < buildingState.Length; i++)
                        {
                            if (buildingState[i] == EBuildingProtesterState.None || buildingState[i] == EBuildingProtesterState.Flower)
                            {
                                flowerPoint = flowerPoints[i];
                                break;
                            }
                        }
                    }

                }

            }

            if(flowerPoint == _buildingCon.DumpFlowerPoint)
            {
                return;
            }

            // Set Destination
            {
                StartCoroutine(IECheckIsZoneComplite());

                Vector3 destinationPosition = flowerPoint.transform.position;
                _innerbuildingProtestingTime = _buildingProtestingTime;

                _navMeshAgent.SetDestination(destinationPosition);
            }


        }

        IEnumerator IECheckIsZoneComplite()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);

                bool isZoneComplite = _buildingCon.IsZoneComplite((int)_zoneNumber);

                if(isZoneComplite == true)
                {
                    SetDestination();
                }

            }
        }

        private void SetDestinationToInitilize()
        {
            _navMeshAgent.SetDestination(Vector3.zero);
            _innerbuildingProtestingTime = _buildingProtestingTime;
        }

        private void SetDestinationToEnd()
        {
            _navMeshAgent.SetDestination(Vector3.zero);
            _innerbuildingProtestingTime = _buildingProtestingTime;
            _isSettedEndDestination = true;
            
        }

        [Button]
        private void SetProtestersCondition(ECivilionState state)
        {
            foreach (var protestor in _toggleProtestors)
            {
                protestor.OnToggleProtestor(state);
            }
        }

    } 
}
