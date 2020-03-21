# json-serializer
A basic JSON Serializer/Deserializer in C#. Doesn't do anything fancy, just turns JSON into dictionaries, lists, strings, and bools. 

Example:
```cs
using Json;
using System;

namespace Example
{
    class Program
    {
        public static void Main(string[] args)
        {
            var data = JSON.Deserialize(@"{""key"": true}");

            Console.WriteLine(data["key"]); // 'True'
        }
    }
}
```
