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

    public class Building_Common : SerializedMonoBehaviour
    {
        public EBuildingProtesterState BuildingState => _state;

        [TitleGroup("PreDefine")]
        [SerializeField] private EBuildingProtesterState _state;
        [SerializeField] private Material _stateNoneMat;
        [SerializeField] private Material _stateFlowerMat;
        [SerializeField] private Material _statePortestMat;
        [SerializeField] MeshRenderer _meshRenderer;

        [TitleGroup("Debug")]
        [SerializeField] Building_Controller _buildingController;
        [SerializeField] float _settedTime;
        [SerializeField] float _remainTime;

        // Start is called before the first frame update
        void Start()
        {
            _buildingController = Building_Controller.Instance;
            _buildingController.AddBuildingInList(this, _state);

            if(_meshRenderer == null)
            {
                _meshRenderer = this.GetComponent<MeshRenderer>();
            }
        }

        private void OnDestroy()
        {
            _buildingController.DeleteBuildingInList(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void UpdateBuildingMaterial(EBuildingProtesterState state)
        {
            switch (state)
            {
                case EBuildingProtesterState.None:
                    _meshRenderer.material = _stateNoneMat;
                    break;
                case EBuildingProtesterState.Flower:
                    _meshRenderer.material = _stateFlowerMat;
                    break;
                case EBuildingProtesterState.Protest:
                    _meshRenderer.material = _statePortestMat;
                    break;
                default:
                    break;
            }
        }

        [Button]
        public void SetProtesterCondition(EBuildingProtesterState state)
        {
            _state = state;
            UpdateBuildingMaterial(state);
            _settedTime = Time.realtimeSinceStartup;

        }

    }

}