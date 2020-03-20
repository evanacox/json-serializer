using System.Collections.Generic;

namespace Serialization
{
    public static class Json
    {
        public static dynamic Deserialize(string data)
        {
            var handler = new Deserializer(data);

            return handler.Deserialize();
        }
    }
}
