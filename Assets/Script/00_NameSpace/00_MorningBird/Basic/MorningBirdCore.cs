using System;
using System.Collections;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using System.ComponentModel;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
/*
/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on the editor, or null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>

//PlayerSettings의 PreLoadedAssets에 등록되어 있지 않으면 작동 안함.
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {

                //에디터상에선 preloaded Assets가 Player Settings를 클릭하기 전까지 작동 안한다.. 요렇게 한줄 넣어주면 해결
#if UNITY_EDITOR
                PlayerSettings.GetPreloadedAssets();
#endif
                _instance = (T)Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            }


            return _instance;
        }
    }
}
*/
namespace MorningBird
{

    public static class CoreMB
    {
        public static float GetVolumeOfColl(BoxCollider coll)
        {
            return coll.size.x * coll.size.y * coll.size.z;
        }
        public static float GetVolumeOfColl(SphereCollider coll)
        {
            return coll.radius * coll.radius * coll.radius * Mathf.PI * 1.333f; // 구의 넓이 구하는 공식, 4/3 * PI * r^3
        }
        public static float GetVolumeOfColl(CapsuleCollider coll)
        {
            return coll.height * (coll.radius * coll.radius * Mathf.PI) + (coll.radius * coll.radius * coll.radius * Mathf.PI * 1.333f);
        }
        /// <summary>
        /// !!!Get Bound of Meshcollider!!!
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static float GetVolumeOfColl(MeshCollider coll)
        {
            return coll.bounds.size.x * coll.bounds.size.y * coll.bounds.size.z;
        }
        /// <summary>
        /// Get Volume of mesh. Mesh must be closed! And submesh will not calculate correctly. And volume of tree such as an chunk with lot of leafs, will not be correct.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        /// If you wanna more information, http://chenlab.ece.cornell.edu/Publication/Cha/icip01_Cha.pdf by Cha Zhang 
        public static float GetVolumeOfMesh(Mesh mesh)
        {

            return VolumeOfMesh(mesh);

            float VolumeOfMesh(Mesh mesh)
            {
                float volume = 0;

                float3[] vertices = new float3[mesh.vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = mesh.vertices[i];
                }

                int[] triangles = mesh.triangles;

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    float3 p1 = vertices[triangles[i + 0]];
                    float3 p2 = vertices[triangles[i + 1]];
                    float3 p3 = vertices[triangles[i + 2]];
                    volume += SignedVolumeOfTriangle(p1, p2, p3);
                }
                return Math.Abs(volume);
            }

            float SignedVolumeOfTriangle(float3 p1, float3 p2, float3 p3)
            {
                float v321 = p3.x * p2.y * p1.z;
                float v231 = p2.x * p3.y * p1.z;
                float v312 = p3.x * p1.y * p2.z;
                float v132 = p1.x * p3.y * p2.z;
                float v213 = p2.x * p1.y * p3.z;
                float v123 = p1.x * p2.y * p3.z;

                return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
            }
        }
    }

    public static class MathMB
    {
        public static float3 ConstForward = new float3(0f, 0f, 1f);
        public static float3 ConstLeft = new float3(-1f, 0f, 0f);

        /// <summary>
        /// Round value to number position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float MathRoundTo(float value, int number)
        {
            for (int intA = 0; intA < number; intA++)
            {
                value *= 10;
            }

            value = (float)Math.Round(value);

            for (int intA = 0; intA < number; intA++)
            {
                value *= 0.1f;
            }
            return value;
        }

        /// <summary>
        /// 최소값과 최대값을 확인해서, Value의 비율을 반전을 시키고자 합니다. value의 minRange와 MaxRange 사이에서의 백분율을 구한 후, 이를 반환합니다.
        /// 이 함수는 Value의 비율을 구하고, 그 비율을 0.5를 기준으로 0~1.0 사이에서 반전합니다. 예컨데 비율의 값이 "0.2라면 0.8을", "0.7이라면, 0.3을" 반환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <returns></returns>
        public static float GetRatioReflectWithValue(float value, float minMotherValue, float maxMotherValue)
        {
            // Set Retrun Checker before start to calculate for defending error occure
            if (value < minMotherValue || value > maxMotherValue || minMotherValue > maxMotherValue)
                return float.NaN;

            // Value의 백분율을 구합니다.
            float tempFloatRatioOfValue = GetRatioValue(value - minMotherValue, maxMotherValue - minMotherValue);

            // 이를 반전합니다.
            if (tempFloatRatioOfValue > 0.5f)
                return 1f - tempFloatRatioOfValue;
            else if (tempFloatRatioOfValue < 0.5f)
                return Math.Abs(tempFloatRatioOfValue - 1f);
            else
                return tempFloatRatioOfValue;
        }

        /// <summary>
        /// 받은 값을 반전합니다. 만약 값이 1.0 초과 혹은 0.0 미만이라면 float.NaN을 반환합니다.
        /// 그 값을 0.5를 기준으로 0~1.0 사이에서 반전합니다. 예컨데 값이 "0.2라면 0.8을", "0.7이라면, 0.3을" 반환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetRatioReflect(float value)
        {
            if (value < 0f || value > 1f)
                return float.NaN;

            if (value > 0.5f)
                return 1f - value;
            else if (value < 0.5f)
                return Math.Abs(value - 1f);
            else
                return value;
        }

        /// <summary>
        /// 최소값과 최대값 안에서 targetValue의 비율을 구합니다. 이 함수는 3과 7 사이에 6은 몇퍼센트인가? 를 구할때 유용합니다.
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        public static float GetRatioValueWithMinimum(float targetValue, float minimumValue, float maximumValue)
        {
            return GetRatioValue(targetValue - minimumValue, maximumValue - minimumValue);
        }

        /// <summary>
        /// Get Ratio(비율) of convertValue. motherValue will divide convertValue. It will return ratio of where convertValue depending on motherValue.
        /// </summary>
        /// <param name="convertValue">It will be divided by motherValue. Which meaning motherValue show where of value.</param>
        /// <param name="motherValue">It will divide convertValue. Whice meaning motherValue show position of ratio.</param>
        /// <returns></returns>
        public static float GetRatioValue(float convertValue, float motherValue)
        {
            return convertValue / motherValue;
        }

        #region GetVectorTotal

        /// <summary>
        /// Get vector elements total. All of elemets use absolute value only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetAbsVectorTotal(Vector2 value)
        {
            return Math.Abs(value.x) + Math.Abs(value.y);
        }

        /// <summary>
        /// Get vector elements total. All of elemets use absolute value only.
        /// </summary>
        /// <param name="value"> Converting to 1 </param>
        /// <returns></returns>
        public static float GetAbsVectorTotal(Vector3 value)
        {
            return Math.Abs(value.x) + Math.Abs(value.y) + Math.Abs(value.z);
        }

        /// <summary>
        /// Get vector elements total. All of elemets use absolute value only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetAbsVectorTotal(float2 value)
        {
            return Math.Abs(value.x) + Math.Abs(value.y);
        }

        /// <summary>
        /// Get vector elements total. All of elemets use absolute value only.
        /// </summary>
        /// <param name="value"> Converting to 1 </param>
        /// <returns></returns>
        public static float GetAbsVectorTotal(float3 value)
        {
            return Math.Abs(value.x) + Math.Abs(value.y) + Math.Abs(value.z);
        }

        /// <summary>
        /// Get vector elements total. If you want absolute value, use instead of GetVectorTotalWithAbsolute. This function is just add all vector elements to an float value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetVectorTotal(Vector2 value)
        {
            return value.x + value.y;
        }

        /// <summary>
        /// Get vector elements total. If you want absolute value, use instead of GetVectorTotalWithAbsolute. This function is just add all vector elements to an float value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetVectorTotal(Vector3 value)
        {
            return value.x + value.y + value.z;
        }

        #endregion


        /// <summary>
        /// Return Cube Root of value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Cbrt(float value)
        {
            return (float)(Math.Pow(value, (double)1 / 3));
        }
        /// <summary>
        /// Return Cube Root of value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Cbrt(double value)
        {
            return Math.Pow(value, (double)1 / 3);
        }

        /// <summary>
        /// sign의 부호를 target에 적용합니다. 예를 들어 sign이 음수라면, target도 음수가 됩니다. sign이 양수라면 target도 양수가 됩니다. 
        /// sign이 0인 경우에는 양수를 반환합니다. 부호있는 0은 취급하지 않습니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static float CopySign(float target, float sign)
        {
            if(target >= 0f)
            {
                if(sign >= 0f)
                { 
                    return target; 
                }
                else if(sign < 0f)
                {
                    return target * -1f;
                }
            }
            else
            {
                if (sign >= 0f)
                {
                    return target * -1f;
                }
                else if (sign < 0f)
                {
                    return target;
                }
            }
            return target;
        }

        public static float3 GetEulerRotation(quaternion quaternion)
        {
            // 1. 오일러엥글을 호도법으로 바꾸고, 57.29578f를 곱한다.
            // 2. 그것을 MakePositive를 한다. 이를 리턴한다.
            // 왜 이렇게 하는지는 모르겠으나, 유니티에서 사용하는 형태이다.
            // 유니티는 쿼터니온을 오일러 엥글로, 그리고 호도법으로, 57.29578f를 곱하고, Positive로 바꾼다.
            // 왤까? 하지만 일단 이렇게 한다.
            return Internal_MakePositive(ToEulerAngles(quaternion)/* * 57.29578f*/);
        }

        /// <summary>
        /// 유니티에서 정규적으로 활용하는 형태는 GetEulerRotaion이다. 따로 명확한 사유가 없다면 GetEulerRotation을 사용하자.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static float3 ToEulerAngles(Quaternion q)
        {
            float3 angles;

            // roll (x-axis rotation)
            float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
            float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
            angles.x = math.atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            float sinp = 2 * (q.w * q.y - q.z * q.x);
            if (math.abs(sinp) >= 1)
                angles.y = CopySign(math.PI * 0.5f, sinp); // use 90 degrees if out of range
            else
                angles.y = math.asin(sinp);

            // yaw (z-axis rotation)
            float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
            float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
            angles.z = math.atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public static float3 QuaternionToDirection(quaternion q)
        {
            return math.mul(q, ConstForward);
        }

        public static float3 QuaternionToDirection(quaternion q, float3 standard)
        {
            return math.mul(q, standard);
        }

        private static float3 Internal_MakePositive(float3 euler)
        {
            float num = -0.005729578f;
            float num2 = 360f + num;
            if (euler.x < num)
            {
                euler.x += 360f;
            }
            else if (euler.x > num2)
            {
                euler.x -= 360f;
            }

            if (euler.y < num)
            {
                euler.y += 360f;
            }
            else if (euler.y > num2)
            {
                euler.y -= 360f;
            }

            if (euler.z < num)
            {
                euler.z += 360f;
            }
            else if (euler.z > num2)
            {
                euler.z -= 360f;
            }

            return euler;
        }

        public static float3 EulerToRad(float3 euler)
        {
            return math.radians(euler);
        }
    }
}
