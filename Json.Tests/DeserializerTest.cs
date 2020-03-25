using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Json.Tests
{
    [TestFixture]
    public class DeserializerTest
    {
        [Test]
        public void Types()
        {
            // strings
            Assert.AreEqual("test", JSON.Deserialize(@"""test"""), "String not deserialized!");

            // numbers
            Assert.AreEqual(5, JSON.Deserialize("5"), "Integers not deserialized!");

            // floats
            Assert.AreEqual(3.333, JSON.Deserialize("3.333"), "Floats not deserialized!");
        }


        [Test]
        public void Lists()
        {
            // simple list
            var resOne = JSON.Deserialize("[1, 2, 3]") as List<dynamic>;
            Assert.That(resOne[0] == 1 && resOne[1] == 2 && resOne[2] == 3,
                "List<int> not deserialized!"
            );

            var resTwo = JSON.Deserialize("[true,false,true]") as List<dynamic>;
            Assert.That(resTwo[0] && !resTwo[1] && resTwo[2],
                "List<bool> not deserialized!"
            );

            var resThree = JSON.Deserialize(@"[{""test"":5,""other"":true}]") as List<dynamic>;
            Assert.That(resThree[0]["test"] == 5 && resThree[0]["other"] == true,
                "List<Dictionary> not deserialized!"
            );

            var resultFour = JSON.Deserialize(@"[ [ [ true ] ] ]") as List<dynamic>;
            Assert.That(resultFour[0][0][0],
                "List<List<List<bool>>> not deserialized!"
            );
        }

        [Test]
        public void Dictionaries()
        {
            var resOne = JSON.Deserialize(@"{""field"": 5, ""otherField"": ""test""}") as Dictionary<string, dynamic>;
            Assert.That(resOne["field"] == 5 && resOne["otherField"] == "test",
                "Dictionary not deserialized!"
            );

            var resTwo = JSON.Deserialize(
                @"{""data"": {""name"": ""James"", ""age"": 19, ""pay"": 1125}, ""lol"": 10}") as Dictionary<string, dynamic>;

            Assert.That(resTwo["data"]["name"] == "James"
                    && resTwo["data"]["age"] == 19
                    && resTwo["data"]["pay"] == 1125
                    && resTwo["lol"] == 10,
                "Dictionary not deserialized!"
            );
        }
    }
}
