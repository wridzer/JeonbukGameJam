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

//PlayerSettings�� PreLoadedAssets�� ��ϵǾ� ���� ������ �۵� ����.
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {

                //�����ͻ󿡼� preloaded Assets�� Player Settings�� Ŭ���ϱ� ������ �۵� ���Ѵ�.. �䷸�� ���� �־��ָ� �ذ�
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
            return coll.radius * coll.radius * coll.radius * Mathf.PI * 1.333f; // ���� ���� ���ϴ� ����, 4/3 * PI * r^3
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
        /// �ּҰ��� �ִ밪�� Ȯ���ؼ�, Value�� ������ ������ ��Ű���� �մϴ�. value�� minRange�� MaxRange ���̿����� ������� ���� ��, �̸� ��ȯ�մϴ�.
        /// �� �Լ��� Value�� ������ ���ϰ�, �� ������ 0.5�� �������� 0~1.0 ���̿��� �����մϴ�. ������ ������ ���� "0.2��� 0.8��", "0.7�̶��, 0.3��" ��ȯ�մϴ�.
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

            // Value�� ������� ���մϴ�.
            float tempFloatRatioOfValue = GetRatioValue(value - minMotherValue, maxMotherValue - minMotherValue);

            // �̸� �����մϴ�.
            if (tempFloatRatioOfValue > 0.5f)
                return 1f - tempFloatRatioOfValue;
            else if (tempFloatRatioOfValue < 0.5f)
                return Math.Abs(tempFloatRatioOfValue - 1f);
            else
                return tempFloatRatioOfValue;
        }

        /// <summary>
        /// ���� ���� �����մϴ�. ���� ���� 1.0 �ʰ� Ȥ�� 0.0 �̸��̶�� float.NaN�� ��ȯ�մϴ�.
        /// �� ���� 0.5�� �������� 0~1.0 ���̿��� �����մϴ�. ������ ���� "0.2��� 0.8��", "0.7�̶��, 0.3��" ��ȯ�մϴ�.
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
        /// �ּҰ��� �ִ밪 �ȿ��� targetValue�� ������ ���մϴ�. �� �Լ��� 3�� 7 ���̿� 6�� ���ۼ�Ʈ�ΰ�? �� ���Ҷ� �����մϴ�.
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
        /// Get Ratio(����) of convertValue. motherValue will divide convertValue. It will return ratio of where convertValue depending on motherValue.
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
        /// sign�� ��ȣ�� target�� �����մϴ�. ���� ��� sign�� �������, target�� ������ �˴ϴ�. sign�� ������ target�� ����� �˴ϴ�. 
        /// sign�� 0�� ��쿡�� ����� ��ȯ�մϴ�. ��ȣ�ִ� 0�� ������� �ʽ��ϴ�.
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
            // 1. ���Ϸ������� ȣ�������� �ٲٰ�, 57.29578f�� ���Ѵ�.
            // 2. �װ��� MakePositive�� �Ѵ�. �̸� �����Ѵ�.
            // �� �̷��� �ϴ����� �𸣰�����, ����Ƽ���� ����ϴ� �����̴�.
            // ����Ƽ�� ���ʹϿ��� ���Ϸ� ���۷�, �׸��� ȣ��������, 57.29578f�� ���ϰ�, Positive�� �ٲ۴�.
            // �ͱ�? ������ �ϴ� �̷��� �Ѵ�.
            return Internal_MakePositive(ToEulerAngles(quaternion)/* * 57.29578f*/);
        }

        /// <summary>
        /// ����Ƽ���� ���������� Ȱ���ϴ� ���´� GetEulerRotaion�̴�. ���� ��Ȯ�� ������ ���ٸ� GetEulerRotation�� �������.
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
