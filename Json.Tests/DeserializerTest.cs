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
    }
}
