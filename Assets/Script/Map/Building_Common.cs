using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MorningBird;

namespace Game.Building
{


    public class Building_Common : Building_Mohter_Common
    {

        [TitleGroup("PreDefine")]
        [SerializeField] private Material _stateNoneMat;
        [SerializeField] private Material _stateFlowerMat;
        [SerializeField] private Material _statePortestMat;
        [SerializeField] MeshRenderer _meshRenderer;



        // Start is called before the first frame update
        void Start()
        {
            if(_meshRenderer == null)
            {
                _meshRenderer = this.GetComponent<MeshRenderer>();
            }

            UpdateBuildingMaterial(_state);
        }

        private void OnDestroy()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void UpdateBuildingMaterial(EBuildingProtesterState state)
        {
            base.UpdateBuildingMaterial(state);

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



    }

}