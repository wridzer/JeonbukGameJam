using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MorningBird;
using UnityEngine.AI;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using Game.Building;

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


        [TitleGroup("Debug")]
        [SerializeField] Game.Building.Building_Controller _buildingCon;
        [SerializeField] Building_Common _selectedBuildingCommon;
        [SerializeField] float _innerbuildingProtestingTime;
        [SerializeField] ECivilionState _civilionState = ECivilionState.Protester;

        [SerializeField] bool _isNPCReachToBuilding = false;
        [SerializeField] float _destinationDistance;
        [SerializeField] float _navMeshMagnitude;
        [SerializeField] float _transformMagnitude;



        void Start()
        {
            if(_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            _buildingCon = Game.Building.Building_Controller.Instance;

            StartCoroutine(IERFD());
        }

        IEnumerator IERFD()
        {
            yield return new WaitForSeconds(3f);
            SetDestination();
        }

        void Update()
        {

            if(CheckReachToBuilding() == true && CheckTimeIsDone() == true)
            {
                DoProtestingWork();
            }
        }


        private void OnTriggerStay(Collider other)
        {

            FindBuildingAndCheckSame(other);

            void FindBuildingAndCheckSame(Collider other)
            {
                if (other.transform.TryGetComponent<Building_Common>(out Building_Common buildingCommon) == true)
                {
                    if (buildingCommon == _selectedBuildingCommon)
                    {
                        return;
                    }

                    _selectedBuildingCommon = buildingCommon;
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
            if (_selectedBuildingCommon == null)
                return false;

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
                    ProtestToBuilding();
                    break;
                default:
                    break;
            }
        }

        private void ProtestToBuilding()
        {
            Collider[] collliders = Physics.OverlapSphere(transform.position, 5f);
            Building_Common t_buildingCommon;

            for (int i = 0; i < collliders.Length; i++)
            {
                if(collliders[i].TryGetComponent<Building_Common>(out t_buildingCommon) == true)
                {
                    t_buildingCommon.SetProtesterCondition(EBuildingProtesterState.Protest);
                    break;
                }
            }

            if(_innerbuildingProtestingTime < 1f)
            {
                Debug.LogError("Time for building is exceed");
            }

            StartCoroutine(IERFD());

        }

        [Button]
        private void FindHome()
        {
            Building.Building_Common[] listOfBuilding = _buildingCon.GetStateBuildings(Building.EBuildingProtesterState.Protest);
            Building.Building_Common oneBuilding = listOfBuilding[Random.Range(0, listOfBuilding.Length)];
            Vector3 destinationPosition = oneBuilding.transform.position;
            destinationPosition.y = 0;

            _navMeshAgent.SetDestination(destinationPosition);
        }

        [Button]
        private void SetDestination()
        {
            Building.Building_Common[] listOfBuilding = _buildingCon.GetStateBuildings(Building.EBuildingProtesterState.None);
            Building.Building_Common oneBuilding = listOfBuilding[Random.Range(0, listOfBuilding.Length)];
            Vector3 destinationPosition = oneBuilding.transform.position;
            destinationPosition.y = 0;

            _navMeshAgent.SetDestination(destinationPosition);
        }

    } 
}
