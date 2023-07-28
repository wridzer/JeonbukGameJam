using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

namespace MorningBird
{

    [BurstCompile]
    public class TimeSafer: MonoBehaviour
    {
        #region Singletone

        private static TimeSafer _timeSafer;

        /// <summary>
        /// TimeSafer
        /// </summary>
        public static TimeSafer TS
        {
            get
            {
                if (_timeSafer == null)
                {
                    TimeSafer obj = FindObjectOfType<TimeSafer>();

                    if (obj != null)
                    {
                        _timeSafer = obj;
                    }
                    else
                    {
                        var newSingleton = new GameObject("TimeSafer").AddComponent<TimeSafer>();
                        _timeSafer = newSingleton;
                    }
                }
                return _timeSafer;
            }
        }

        #endregion
        private void Awake()
        {
            #region Singleton Instantiate
            var objs = FindObjectsOfType<TimeSafer>();
            if (objs.Length > 1)
            {
                Debug.LogError("New TimeSafer Added And Destroy Automatically");
                Destroy(this.gameObject);
                return;
            }
            #endregion
        }

        static float[] _times = new float[16];
        static float[] _stands = { 0.02f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.35f, 0.495f, 0.75f, 1.005f, 1.495f, 1.995f, 3f, 5f, 7.5f, 10.15f };
        static bool[] _safers = new bool[16];
        static float[] _fixedTimes = new float[16];
        static bool[] _fixedSafers = new bool[16];

        #region Safer Getter

        public bool Get20msSafer => _safers[0];
        public bool Get50msSafer => _safers[1];
        public bool Get100msSafer => _safers[2];
        public bool Get150msSafer => _safers[3];
        public bool Get200msSafer => _safers[4];
        public bool Get250msSafer => _safers[5];
        public bool Get350msSafer => _safers[6];
        public bool Get495msSafer => _safers[7];
        public bool Get750msSafer => _safers[8];
        public bool Get1005msSafer => _safers[9];
        public bool Get1495msSafer => _safers[10];
        public bool Get1995msSafer => _safers[11];
        public bool Get3000msSafer => _safers[12];
        public bool Get5000msSafer => _safers[13];
        public bool Get7500msSafer => _safers[14];
        public bool Get10150msSafer => _safers[15];

        #endregion

        #region FixedSafer Getter

        public bool GetFixed20msSafer => _fixedSafers[0];
        public bool GetFixed50msSafer => _fixedSafers[1];
        public bool GetFixed100msSafer => _fixedSafers[2];
        public bool GetFixed150msSafer => _fixedSafers[3];
        public bool GetFixed200msSafer => _fixedSafers[4];
        public bool GetFixed250msSafer => _fixedSafers[5];
        public bool GetFixed350msSafer => _fixedSafers[6];
        public bool GetFixed495msSafer => _fixedSafers[7];
        public bool GetFixed750msSafer => _fixedSafers[8];
        public bool GetFixed1005msSafer => _fixedSafers[9];
        public bool GetFixed1495msSafer => _fixedSafers[10];
        public bool GetFixed1995msSafer => _fixedSafers[11];
        public bool GetFixed3000msSafer => _fixedSafers[12];
        public bool GetFixed5000msSafer => _fixedSafers[13];
        public bool GetFixed7500msSafer => _fixedSafers[14];
        public bool GetFixed10150msSafer => _fixedSafers[15];

        #endregion


        // Update is called once per frame
        void Update()
        {
            AddTime();
            CheckTime();
        }

        private void FixedUpdate()
        {
            AddFixedTime();
            CheckFixedTime();
        }

        static void AddTime()
        {
            for (int i = 0; i < _times.Length; i++)
            {
                _times[i] += Time.deltaTime;
            }
        }

        static void CheckTime()
        {
            for (int i = 0; i < _times.Length; i++)
            {
                if(_times[i] >= _stands[i])
                {
                    _safers[i] = true;
                    _times[i] = 0;
                }
                else
                {
                    _safers[i] = false;
                }
            }
        }

        static void AddFixedTime()
        {
            for (int i = 0; i < _fixedTimes.Length; i++)
            {
                _fixedTimes[i] += Time.deltaTime;
            }
        }

        static void CheckFixedTime()
        {
            for (int i = 0; i < _fixedTimes.Length; i++)
            {
                if (_fixedTimes[i] >= _stands[i])
                {
                    _fixedSafers[i] = true;
                    _fixedTimes[i] = 0;
                }
                else
                {
                    _fixedSafers[i] = false;
                }
            }
        }
    }

}
