using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Building
{
    public class Building_FlowerPoint : SerializedMonoBehaviour
    {
        [TitleGroup("PreDefine")]
        [SerializeField] Transform _showUpTrans;
        [SerializeField] EBuildingProtesterState _state;

        [TitleGroup("Debug")]
        [SerializeField] Building_Mohter_Common[] _buildingCommons;
        [SerializeField] Building_Controller _buildingController;
        [SerializeField] float _settedTime;
        [SerializeField] float _remainTime;



        // Start is called before the first frame update
        void Start()
        {
            _buildingCommons = _showUpTrans.GetComponentsInChildren<Building_Mohter_Common>();
            _buildingController = Building_Controller.Instance;
            _buildingController.AddBuildingInList(this, _state);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            _buildingController.DeleteBuildingInList(this);
        }

        [Button]
        public void SetFlowerPointCondition(EBuildingProtesterState state)
        {
            _state = state;
            for (int i = 0; i < _buildingCommons.Length; i++)
            {
                _buildingCommons[i].UpdateBuildingMaterial(state);
            }

            _buildingController.SetBuildingState(this, state);
            _settedTime = Time.realtimeSinceStartup;

        }
    }
}