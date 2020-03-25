using System.Text.RegularExpressions;
using System.Collections.Generic;
using NUnit.Framework;
using System.Numerics;

namespace Json.Tests
{
    [TestFixture]
    public class SerializerTest
    {
        [Test]
        public void Types()
        {
            // strings
            Assert.AreEqual(@"""string""", JSON.Serialize("string"), "String not serialized!");

            // regular integers
            Assert.AreEqual(@"-49", JSON.Serialize(-49), "String not serialized!");

            // floating point numbers
            Assert.AreEqual(@"6.66666666", JSON.Serialize(6.66666666), "Decimal not serialized!");

            // BigIntegers
            Assert.AreEqual(@"58340948309860325902",
                JSON.Serialize(BigInteger.Parse("58340948309860325902")),
                "BigInteger not serialized!"
            );

            // ISerializables
            var complex = new Point(1, 1);
            Assert.AreEqual(@"""(1, 1)""", JSON.Serialize(complex), "ISerializable (Point) not serialized!");

            var example = new ExampleObject("James", 13, 1150);
            Assert.AreEqual(@"{""name"":""James"",""age"":13,""weeklyPay"":1150}",
                JSON.Serialize(example),
                "ISerializable (ExampleObject) not serialized!"
            );
        }

        [Test]
        public void Lists()
        {
            // test simple number list
            var listOne = new List<int> { 1, 2, 3, 4, 5 };
            Assert.AreEqual(@"[1,2,3,4,5]", JSON.Serialize(listOne), "Integer list not serialized!");

            // test simple bool list
            var listTwo = new List<bool> { true, false, true, true };
            Assert.AreEqual(@"[true,false,true,true]", JSON.Serialize(listTwo), "Boolean list not serialized!");

            // test simple string list
            var listThree = new List<string> { "this", "is", "a", "test" };
            Assert.AreEqual(@"[""this"",""is"",""a"",""test""]", JSON.Serialize(listThree), "String list not serialized!");

            // test a mixed numeric type list
            var listFour = new List<object> { 1, 2, 2.5, 3, BigInteger.Parse("44444444444444") };
            Assert.AreEqual(@"[1,2,2.5,3,44444444444444]", JSON.Serialize(listFour), "Mixed numeric list not serialized!");

            // test a mixed type list
            var listFive = new List<object> { true, 2.645, "three", false, BigInteger.Parse("5") };
            Assert.AreEqual(@"[true,2.645,""three"",false,5]", JSON.Serialize(listFive), "Mixed type list not serialized!");

            // test a list with hashtables inside it
            var listSix = new List<object>
            {
                new Dictionary<string, object>
                {
                    ["one"] = 1,
                    ["two"] = 2,
                    ["three"] = 3,
                    ["four"] = new List<int> { 1, 2, 3, 4 }
                },
                new Dictionary<string, object>
                {
                    ["test"] = true,
                    ["name"] = "James",
                    ["decimal"] = (decimal)3.33333,
                },
            };

            var resultString = "["
                + @"{""one"":1,""two"":2,""three"":3,""four"":[1,2,3,4]},"
                + @"{""test"":true,""name"":""James"",""decimal"":3.33333}"
                + "]";
            Assert.AreEqual(resultString, JSON.Serialize(listSix), "Hashtable list not serialized!");
        }

        [Test]
        public void Dictionaries()
        {
            var dictOne = new Dictionary<string, object>
            {
                ["test"] = true,
                ["name"] = "James",
                ["decimal"] = (decimal)3.33333,
            };

            var resultOne = @"{""test"":true,""name"":""James"",""decimal"":3.33333}";

            Assert.AreEqual(resultOne, JSON.Serialize(dictOne), "Simple Hashtable not serialized!");

            var dictTwo = new Dictionary<string, object>
            {
                ["list"] = new List<object>
                {
                    new Dictionary<string, object>
                    {
                        ["list"] = new List<object> { 1, 2, 3, 4, 5 }
                    }
                }
            };

            var resultTwo = @"{""list"":[{""list"":[1,2,3,4,5]}]}";

            Assert.AreEqual(resultTwo, JSON.Serialize(dictTwo), "Nested Hashtable/List not serialized!");

            var dictThree = new Dictionary<string, object>
            {
                ["fields"] = new Dictionary<string, object>
                {
                    ["foo"] = "test",
                    ["bar"] = 5,
                    ["this"] = 3.3333,
                    ["that"] = false
                },
                ["fieldsOther"] = new Dictionary<string, object>
                {
                    ["foo"] = "test",
                    ["bar"] = 5,
                    ["this"] = 3.3333,
                    ["that"] = false
                }
            };

            var resultThree = "{"
                + @"""fields"":{"
                    + @"""foo"":""test"",""bar"":5,""this"":3.3333,""that"":false"
                + "},"
                + @"""fieldsOther"":{"
                    + @"""foo"":""test"",""bar"":5,""this"":3.3333,""that"":false"
                + "}}";

            Assert.AreEqual(resultThree, JSON.Serialize(dictThree), "Nested hashtables not serialized!");
        }
    }
}
