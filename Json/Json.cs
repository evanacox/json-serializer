namespace Json
{
    /// <summary>
    /// Main JSON class
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// Deserializes a JSON string into a .NET object
        /// </summary>
        /// <param name="data">A JSON string</param>
        /// <returns>A .NET object</returns>
        public static dynamic Deserialize(string data)
        {
            var handler = new Deserializer(data);

            return handler.Deserialize();
        }

        /// <summary>
        /// Turns a .NET object into a JSON string.
        /// 
        /// Valid .NET objects:
        ///   - IDictionary<string, dynamic>
        ///   - IList<dynamic>
        ///   - System.String
        ///   - Any standard integral types, and all three floats
        ///   - BigInteger
        ///   - Anything ISerializable (ISerializable.JsonSerialize() must return valid JSON)
        /// </summary>
        /// <param name="obj">A .NET object</param>
        /// <returns>A JSON string</returns>
        public static string Serialize(dynamic obj)
        {
            var handler = new Serializer();

            return handler.Serialize(obj);
        }
    }
}
