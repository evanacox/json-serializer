using System;
using System.Collections.Generic;
using System.Text;

namespace Json
{
    public interface ISerializable 
    {
        public string JsonSerialize();
    }
}
