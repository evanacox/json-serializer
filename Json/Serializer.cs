using System;
using System.Collections.Generic;
using System.Text;

namespace Json
{
    internal class Serializer
    {
        private dynamic _object;

        public Serializer(dynamic obj) => _object = obj;

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
