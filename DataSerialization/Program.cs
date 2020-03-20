using System;
using System.Collections.Generic;
using Serialization;
 
namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = Json.Deserialize(@"""test""");
            var number = Json.Deserialize(@"5.5");
            var boolean = Json.Deserialize(@"true");

            Console.WriteLine($"Type of 'str'   : '{str.GetType()}', value: '{str}'");
            Console.WriteLine($"Type of 'number': '{number.GetType()}', value: '{number}'");
            Console.WriteLine($"Type of 'boolean': '{boolean.GetType()}', value: '{boolean}'");

            var dict = Json.Deserialize(@"{""test"": true, ""number"": 3258.69, ""string"": ""this is a test""}") as IReadOnlyDictionary<string, dynamic>;
            Console.WriteLine($"Type of 'dict': '{dict.GetType()}',");
            Console.WriteLine($"   value of dict['test']: '{dict["test"]}',");
            Console.WriteLine($"   value of dict['number']: '{dict["number"]}',");
            Console.WriteLine($"   value of dict['string']: '{dict["string"]}',");

        }
    }
}
