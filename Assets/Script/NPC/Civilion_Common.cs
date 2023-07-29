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

            StartCoroutine(IEWaitSetDestination(3f));
        }

        IEnumerator IEWaitSetDestination(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
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
                    ProtestToFlowerPooint();
                    break;
                default:
                    break;
            }
        }

        private void ProtestToFlowerPooint()
        {
            Collider[] collliders = Physics.OverlapSphere(transform.position, 5f);
            Building_FlowerPoint t_buildingCommon;

            for (int i = 0; i < collliders.Length; i++)
            {
                if(collliders[i].TryGetComponent<Building_FlowerPoint>(out t_buildingCommon) == true)
                {
                    t_buildingCommon.SetFlowerPointCondition(EBuildingProtesterState.Protest);
                    break;
                }
            }

            if(_innerbuildingProtestingTime > 1f)
            {
                Debug.LogError("Time for building is exceed");
            }

            StartCoroutine(IEWaitSetDestination(5f));

        }

        [Button]
        private void FindHome()
        {
            Building.Building_FlowerPoint[] listOfBuilding = _buildingCon.GetStateBuildings(Building.EBuildingProtesterState.Protest);
            Building.Building_FlowerPoint oneBuilding = listOfBuilding[Random.Range(0, listOfBuilding.Length)];
            Vector3 destinationPosition = oneBuilding.transform.position;

            _navMeshAgent.SetDestination(destinationPosition);
        }

        [Button]
        private void SetDestination()
        {
            Building.Building_FlowerPoint[] listOfBuilding = _buildingCon.GetFlowerPointForProtester();

            if( listOfBuilding.Length <= 0 ) 
            { 
                _navMeshAgent.SetDestination(Vector3.zero);
                StartCoroutine(IEWaitSetDestination(10f));
                return;

            }

            Building.Building_FlowerPoint oneBuilding = listOfBuilding[Random.Range(0, listOfBuilding.Length)];

            Vector3 destinationPosition = oneBuilding.transform.position;
            destinationPosition.y = 0;
            _innerbuildingProtestingTime = _buildingProtestingTime;

            _navMeshAgent.SetDestination(destinationPosition);
        }

    } 
}
