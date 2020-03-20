using System;
using System.Collections.Generic;
using Json;
 
namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = JSON.Deserialize(@"""test""");
            var number = JSON.Deserialize(@"5.5");
            var boolean = JSON.Deserialize(@"true");
            var array = JSON.Deserialize(@"[true, false, 1, 1.5, ""hi""]") as IReadOnlyList<dynamic>;

            Console.WriteLine($"Type of 'str'   : '{str.GetType()}', value: '{str}'");
            Console.WriteLine($"Type of 'number': '{number.GetType()}', value: '{number}'");
            Console.WriteLine($"Type of 'boolean': '{boolean.GetType()}', value: '{boolean}'");
            Console.WriteLine($"Type of 'array': '{array.GetType()}',");

            ExamineArray(array);

            Object();
        }

        static void Object()
        {
            var dict = JSON.Deserialize(@"
            {
                ""test"": true,

                ""number"": 3258.69,
                ""string"": ""this is a test"",
                ""array"": [""yes"", 55, 69.6969696969],
                ""object"": {
                    ""key"": false,
	                ""integral"": 580394,
	                ""float"": 25923.333333
                },
                ""arrayOfObjects"": [
                    {
                        ""key"": true,
                        ""name"": ""evan""
                    },
                    {
                        ""key"": true,
                        ""name"": ""not evan""
                    }
                ]
            }") as IReadOnlyDictionary<string, dynamic>;

            Console.WriteLine($"Type of 'dict': '{dict.GetType()}',");
            ExamineDict(dict);
        }

        static void ExamineDict(IReadOnlyDictionary<string, dynamic> dict, string prefix = "  ")
        {
            foreach (var (k, v) in dict)
            {
                Console.WriteLine($"{prefix}value of dict['{k}']: '{v}',");

                if (v is IReadOnlyDictionary<string, dynamic>)
                {
                    ExamineDict(v, prefix + "  ");
                }

                else if (v is IReadOnlyList<dynamic>)
                {
                    ExamineArray(v, prefix + "  ");
                }
            }
        }

        static void ExamineArray(IReadOnlyList<dynamic> list, string prefix = "  ")
        {
            foreach (var item in list)
            {
                Console.WriteLine($"{prefix}value: '{item}',");

                if (item is IReadOnlyDictionary<string, dynamic>)
                {
                    ExamineDict(item, prefix + "  ");
                }

                else if (item is IReadOnlyList<dynamic>)
                {
                    ExamineArray(item, prefix + "  ");
                }
            }
        }
    }
}
