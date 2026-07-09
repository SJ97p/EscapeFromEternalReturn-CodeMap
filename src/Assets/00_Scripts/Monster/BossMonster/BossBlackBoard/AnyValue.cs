using System;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [Serializable]
    public struct AnyValue
    {
        public enum valueTpye { Int, Float, Bool, String, Vector3 }
        public valueTpye type;

        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public Vector3 vector3Value;

        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();

        T ConvertValue<T>()
        {
            return type switch
            {
                valueTpye.Int => AsInt<T>(intValue),
                valueTpye.Float => AsFloat<T>(floatValue),
                valueTpye.Bool => AsBool<T>(boolValue),
                valueTpye.String => AsString<T>(stringValue),
                valueTpye.Vector3 => AsVector3<T>(vector3Value),
                _ => throw new NotSupportedException($"Not Supported Value Type {typeof(T)}")
            };
        }

        T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        T AsString<T>(string value) => typeof(T) == typeof(string) && value is T correctType ? correctType : default;
        T AsVector3<T>(Vector3 value) => typeof(T) == typeof(Vector3) && value is T correctType ? correctType : default;
    }
}