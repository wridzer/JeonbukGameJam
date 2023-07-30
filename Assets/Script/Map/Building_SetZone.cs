using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Building
{
    public class Building_SetZone : MonoBehaviour
    {
        [SerializeField] EZoneNumber _zoneNumber;

        void Awake()
        {
            Building_FlowerPoint[] _flowerPoints = this.transform.GetComponentsInChildren<Building_FlowerPoint>();

            foreach (Building_FlowerPoint flowerPoint in _flowerPoints)
            {
                flowerPoint.SetZoneNumber = _zoneNumber;
            }
        }

    }
}