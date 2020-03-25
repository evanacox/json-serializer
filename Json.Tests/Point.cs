namespace Json.Tests
{
    /// <summary>
    /// Another example ISerializable type
    /// </summary>
    struct Point : ISerializable
    {
        private double x;
        private double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        string ISerializable.JsonSerialize()
        {
            return $@"""({x}, {y})""";
        }
    }
}
