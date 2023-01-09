using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utilities
{
    public static class Utilities
    {
        const string circularTransformShaderName = "CircularWorldMath";
        static ComputeShader _circularTransformShader;
        public static ComputeShader circularTransformShader
        {
            get
            {
                if (_circularTransformShader != null)
                {
                    return _circularTransformShader;
                }

                _circularTransformShader = Resources.Load<ComputeShader>("CircularWorldMath");
                return _circularTransformShader;
            }
        }

        static System.Random _R = new System.Random(); 

        public static bool Any<T>(this IList<T> source)
        {
            foreach (var val in source)
            {
                return true;
            }

            return false;
        }

        public static bool Any<T>(this IList<T> source, Func<T, bool> condition, out int index)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (condition(source[i]))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public static float ClampAngle(float angle, float from, float to)
        {
            if (angle < 0f) angle = 360 + angle;
            if (angle > 180f) return Mathf.Max(angle, 360 + from);
            return Mathf.Min(angle, to);
        }

        public static float Circumradius(float sideLength, float numSides)
        {
            return sideLength / (2 * Mathf.Sin(Mathf.PI / numSides));
        }

        public static bool DegreeInMinRange(float angle, float endPoint1, float endPoint2)
        {
            angle = angle % 360;
            endPoint1 = endPoint1 % 360;
            endPoint2 = endPoint2 % 360;

            if (angle < 0) angle += 360;
            if (endPoint1 < 0) endPoint1 += 360;
            if (endPoint2 < 0) endPoint2 += 360;

            if (Mathf.Abs(endPoint2 - endPoint1) < 180)
            {
                return InRange(angle, endPoint1, endPoint2);
            }
            else
            {
                return !InRange(angle, endPoint1, endPoint2);
            }
        }

        public static float DegreesToRads(this float degreesToRad)
        {
            return degreesToRad * Mathf.PI / 180;
        }

        public static Vector2 DegreeToVector2(this float degree)
        {
            return RadToVector2(degree * Mathf.Deg2Rad);
        }

        public static void Destroy(this IEnumerable<UnityEngine.GameObject> objects)
        {
            foreach (var obj in objects)
            {
                GameObject.Destroy(obj.gameObject);
            }
        }

        public static void Destroy(this IEnumerable<UnityEngine.MonoBehaviour> objects)
        {
            foreach (var obj in objects)
            {
                GameObject.Destroy(obj.gameObject);
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate, T defaultValue) where T : class
        {
            foreach (var value in source)
            {
                if (predicate(value))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        public static V GetOrDefault<T, V>(this Dictionary<T, V> keyValuePairs, T key, V defaultValue)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                return keyValuePairs[key];
            }
            return defaultValue;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        public static List<Transform> GetChildrenTransformsRecursive(this Transform transform)
        {
            List<Transform> transforms = new List<Transform>();
            transform.GetChildrenTransformsRecursive(transforms);
            return transforms;
        }

        public static void GetChildrenTransformsRecursive(this Transform transform, List<Transform> childrenList)
        {
            childrenList.Add(transform);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChildrenTransformsRecursive(childrenList);
            }
        }

        public static IEnumerable<Enum> GetEnums(Type type)
        {
            foreach (var e in Enum.GetValues(type))
            {
                Enum ret = e as Enum;
                if (ret == null)
                    continue;

                yield return ret;
            }
        }

        public static IEnumerable<T> GetEnums<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static Vector2 GetMouseFromCenter()
        {
            var mousePos = Input.mousePosition;
            return new Vector2
                (mousePos.x - Screen.width / 2
                , mousePos.y - Screen.height / 2
                ); ;
        }

        public struct Example
        {
            public int id;
            public string name;
            public int count;

            public Example(int id, string name, int count)
            {
                this.id = id;
                this.name = name;
                this.count = count;
            }
        }

        public static Vector3[] HorizontalBounds(this RectTransform transform, Camera camera)
        {
            if (transform is RectTransform rt)
            {
                Vector3[] corners = new Vector3[4];
                rt.GetWorldCorners(corners);

                for (int i = 0; i < corners.Length; i++)
                {
                    corners[i] = camera.WorldToScreenPoint(corners[i]);
                }

                Vector3[] BoundingCorners = new Vector3[2];
                BoundingCorners[0] = corners.Min(x => x.x);
                BoundingCorners[1] = corners.Max(x => x.x);

                return BoundingCorners;
            }

            return new Vector3[0];
        }

        public static int IndexOf<T>(this IList<T> source, Func<T, bool> condition)
        {
            for (int i = 0; i < source.Count; i++)
                if (condition(source[i]))
                    return i;

            return -1;
        }

        public static bool InRange(float value, float e1, float e2)
        {
            if (e1 > e2)
            {
                return e1 > value && e2 < value;
            }
            return e2 > value && e1 < value;
        }

        public static float InRadius(float sideLength, float numSides)
        {
            return sideLength / (2 * Mathf.Tan(Mathf.PI / numSides));
        }

        public static bool Intersects(float leftBound1, float rightBound1, float leftBound2, float rightBound2)
        {
            if (leftBound2 <= rightBound1 && leftBound2 >= leftBound1)
            {
                return true;
            }
            if (rightBound2 <= rightBound1 && rightBound2 >= leftBound1)
            {
                return true;
            }
            if (leftBound1 <= rightBound2 && leftBound1 >= leftBound2)
            {
                return true;
            }
            if (rightBound1 <= rightBound2 && rightBound1 >= leftBound2)
            {
                return true;
            }
            return false;
        }

        public static bool IsAny<T>(this T val, T v1) where T : struct => val.Equals(v1);
        public static bool IsAny<T>(this T val, T v1, T v2) where T : struct => val.Equals(v1) || val.Equals(v2);
        public static bool IsAny<T>(this T val, T v1, T v2, T v3) where T : struct => val.Equals(v1) || val.Equals(v2) || val.Equals(v3);
        public static bool IsAny<T>(this T val, T v1, T v2, T v3, T v4) where T : struct => val.Equals(v1) || val.Equals(v2) || val.Equals(v3) || val.Equals(v4);
        public static bool IsAny<T>(this T val, T v1, T v2, T v3, T v4, T v5) where T : struct => val.Equals(v1) || val.Equals(v2) || val.Equals(v3) || val.Equals(v4) || val.Equals(v5);
        public static bool IsAny<T>(this T val, T v1, T v2, T v3, T v4, T v5, T v6) where T : struct => val.Equals(v1) || val.Equals(v2) || val.Equals(v3) || val.Equals(v4) || val.Equals(v5) || val.Equals(v6);
        public static bool IsAny<T>(this T val, T v1, T v2, T v3, T v4, T v5, T v6, T v7) where T : struct => val.Equals(v1) || val.Equals(v2) || val.Equals(v3) || val.Equals(v4) || val.Equals(v5) || val.Equals(v6) || val.Equals(v7);
        public static bool IsAny<T>(this T val, T v1, T v2, T v3, T v4, T v5, T v6, T v7, T v8) where T : struct => val.Equals(v1) || val.Equals(v2) || val.Equals(v3) || val.Equals(v4) || val.Equals(v5) || val.Equals(v6) || val.Equals(v7) || val.Equals(v8);

        public static float[] LinSpace(float start, float end, int size)
        {
            if (size == 0)
            {
                return new float[0] { };
            }
            
            if (size == 1)
            {
                return new float[1] { start };
            }
            
            if (size == 2)
            {
                return new float[2] { start, end };
            }

            float[] arr = new float[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = LinSpaceIndex(start, end, size, i);
            }
            return arr;
        }

        public static float LinSpaceIndex(float start, float end, int size, int index)
        {
            return start + (end - start) * index / (size - 1);
        }

        public static T Max<T>(this T[] values, Func<T, float> val) where T : struct
        {
            float currentMaxValue = float.MinValue;
            T currentMax = default(T);

            for (int i = 0; i < values.Length; i++)
            {
                var currentValue = val(values[i]);
                if (currentValue > currentMaxValue)
                {
                    currentMaxValue = currentValue;
                    currentMax = values[i];
                }
            }

            return currentMax;
        }

        public static T Min<T>(this T[] values, Func<T, float> val) where T : struct
        {
            float currentMinValue = float.MaxValue;
            T currentMin = default(T);

            for (int i = 0; i < values.Length; i++)
            {
                var currentValue = val(values[i]);
                if (currentValue < currentMinValue)
                {
                    currentMinValue = currentValue;
                    currentMin = values[i];
                }
            }

            return currentMin;
        }

        public static Vector2 PolarToCartesian(float Radius, float Angle)
        {
            return new Vector2(Radius * Mathf.Cos(Angle), Radius * Mathf.Sin(Angle));
        }

        public static Vector2 RadToVector2(this float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static IEnumerable<int> Repeat(this int n)
        {
            for (int i = 0; i < n; i++)
                yield return n;
        }

        public static int Range(this System.Random random, int min, int max)
        {
            return random.Next(min, max);
        }

        public static double Range(this System.Random random, double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        public static T RandomEnum<T>() where T : Enum
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public static T RandomLocalizedEnum<T>() where T : Enum
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public static int[] Range(this int val, int length)
        {
            int[] ret = new int[length - val];
            for (int i = 0; i < length - val; i++)
            {
                ret[i] = i + val;
            }

            return ret;
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static bool SameAs<T>(this T[] array1, T[] array2)
        {
            if (array1.Length != array2.Length) return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] == null && array2[i] == null)
                {
                    continue;
                }
                if (array1[i] == null || array2[i] == null)
                {
                    return false;
                }
                if (array1[i]?.Equals(array2[2]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static void Time(Action action, object funcName)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            action();
            watch.Stop();
            Debug.Log($"{funcName}: {(watch.ElapsedMilliseconds / 1000.0f).ToString("N4")}");
        }

        public static void TransformMeshesToCircle(Transform parentTransform, float circumference, bool recalculateMeshData, int rotationalOffset = 0)
        {
            if (circularTransformShader == null)
            {
                return;
            }

            int totalSize = sizeof(float) * 3;

            Vector3 outValue = Utilities.TransformVertexToPolygon(circumference, parentTransform.position.x, parentTransform.position.y, parentTransform.position.z, rotationalOffset);
            Vector3 difference = outValue - parentTransform.position;
            parentTransform.position = outValue;

            List<Transform> childTransforms = parentTransform.GetChildrenTransformsRecursive();
            foreach (var childTransform in childTransforms)
            {
                if (childTransform.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                {
                    Mesh mesh = meshFilter.mesh;

                    Vector3[] data = mesh.vertices;
                    ComputeBuffer computeBuffer = new ComputeBuffer(mesh.vertexCount, totalSize);
                    computeBuffer.SetData(data);

                    circularTransformShader.SetBuffer(0, "vertex", computeBuffer);
                    circularTransformShader.SetVector("objectOffset", difference);
                    circularTransformShader.SetMatrix("localToWorld", childTransform.localToWorldMatrix);
                    circularTransformShader.SetMatrix("worldToLocal", childTransform.worldToLocalMatrix);
                    circularTransformShader.SetFloat("circumference", circumference);
                    circularTransformShader.SetFloat("rotationalOffset", rotationalOffset);
                    circularTransformShader.Dispatch(0, (int)Mathf.Ceil(mesh.vertexCount / 32.0f), 1, 1);

                    computeBuffer.GetData(data);
                    mesh.SetVertices(data);

                    if (recalculateMeshData)
                    {
                        mesh.RecalculateBounds();
                        mesh.RecalculateNormals();
                        mesh.RecalculateTangents();
                    }

                    computeBuffer.Dispose();
                }
            }
        }

        public static Vector3 TransformVertexToPolygon(float circumference, float x, float y, float z, float rotationalOffset = 0)
        {
            float r = circumference / (2 * Mathf.PI);
            Vector2 values = PolarToCartesian(y + r, -(x - rotationalOffset) / r);
            return new Vector3(values.x, values.y, z * ((circumference * Mathf.Sin(Mathf.PI / circumference)) / Mathf.PI));
        }

        public static Vector2 XZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 XYZ(this Vector4 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }
    }
}