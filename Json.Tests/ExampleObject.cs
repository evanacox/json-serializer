namespace Json.Tests
{
    /// <summary>
    /// This type exists purely for use in tests
    /// </summary>
    struct ExampleObject : ISerializable
    {
        string name;

        uint age;

        uint weeklyPay;

        public ExampleObject(string name, uint age, uint weeklyPay)
        {
            this.name = name;
            this.age = age;
            this.weeklyPay = weeklyPay;
        }

        string ISerializable.JsonSerialize()
        {
            var final = "{";

            final += $@"""name"":""{name}"",";
            final += $@"""age"":{age},";
            final += $@"""weeklyPay"":{weeklyPay}";

            return final + "}";
        }
    }

}
