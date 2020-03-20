using System.Collections.Generic;

namespace Json
{
    public static class JSON
    {
        public static dynamic Deserialize(string data)
        {
            var handler = new Deserializer(data);

            return handler.Deserialize();
        }

        public static string Serialize(dynamic obj)
        {
            var handler = new Serializer(obj);

            return handler.Serialize();
        }
    }
}
