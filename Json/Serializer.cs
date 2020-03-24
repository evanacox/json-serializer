using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Json
{
    /// <summary>
    /// JSON serializer class
    /// </summary>
    internal class Serializer
    {
        /// <summary>
        /// Serializes a .NET object into a JSON string
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>The JSON string</returns>
        public string Serialize(dynamic obj)
        {
            if (obj == null)
                return "null";

            else if (IsSerializableDictionary(obj))
                return SerializeDictionary(obj);

            else if (IsSerializableList(obj))
                return SerializeList(obj);

            else if (IsNumber(obj))
                return SerializeNumber(obj);

            else if (obj is bool)
                return SerializeBoolean(obj);

            else if (obj is string)
                return SerializeString(obj);

            else
                return obj.ToString();
        }

        /// <summary>
        /// Turns a .NET dictionary into a JSON object
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="dict">The dictionary</param>
        /// <returns>Returns a JSON object string</returns>
        public string SerializeDictionary<T>(IDictionary<string, T> dict)
        {
            var final = "{";

            foreach (var (k, v) in dict)
            {
                final += $@"""{k}"":{Serialize(v)},";
            }

            // remove final comma, and append a }
            return final[0..^1] + "}";
        }

        /// <summary>
        /// Turns a .NET List into a JSON array
        /// </summary>
        /// <typeparam name="T">The type of the objects in the list</typeparam>
        /// <param name="list">The list to be converted</param>
        /// <returns>A JSON array</returns>
        public string SerializeList<T>(IList<T> list)
        {
            var final = "[";

            foreach (var item in list)
            {
                final += Serialize(item) + ",";
            }

            return final[0..^1] + "]";
        }

        /// <summary>
        /// Turns a .NET String into a JSON string
        /// </summary>
        /// <param name="str">The string to be converted</param>
        /// <returns>The JSON string</returns>
        public string SerializeString(string str)
        {
            return '"' + str + '"';
        }

        /// <summary>
        /// Turns a .NET boolean into a JSON boolean
        /// </summary>
        /// <param name="boolean">The .NET bool object</param>
        /// <returns>The JSON boolean</returns>
        public string SerializeBoolean(bool boolean)
        {
            return boolean.ToString().ToLower();
        }

        /// <summary>
        /// Turns a .NET number into a JSON number
        /// </summary>
        /// <param name="n">The number to be converted</param>
        /// <returns>The JSON number</returns>
        public string SerializeNumber(object n)
        {
            return n.ToString();
        }

        /// <summary>
        /// Serializes an already-serializable object
        /// </summary>
        /// <param name="obj">An ISerializable object</param>
        /// <returns>The object as a JSON string</returns>
        public string SerializeSerializable(ISerializable obj)
        {
            return obj.JsonSerialize();
        }

        /// <summary>
        /// Checks if an object is some type of .NET number
        /// </summary>
        /// <param name="n">The number</param>
        /// <returns>Whether the object is a .NET number</returns>
        private static bool IsNumber(dynamic n)
        {
            return n is int
                || n is long
                || n is short
                || n is float
                || n is double
                || n is uint
                || n is ulong
                || n is ushort
                || n is byte
                || n is sbyte
                || n is decimal
                || n is BigInteger;
        }

        /// <summary>
        /// Returns if an object is a serializable .NET dictionary
        /// 
        /// JSON objects need to be string key values, so the type of the
        /// key has to be a string. The value just needs to be serializable. 
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>Whether the object is a serializable dictionary</returns>
        private static bool IsSerializableDictionary(dynamic obj)
        {
            // its not worth testing further if its not a Dictionary
            if (obj is IDictionary dict)
            {
                var args = dict.GetType().GetGenericArguments();

                // check if key type is string
                return args[0] == typeof(string);
            }

            return false;
        }

        /// <summary>
        /// Returns whether an object is a serializable .NET list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsSerializableList(dynamic obj) => obj is IList;
    }
}
