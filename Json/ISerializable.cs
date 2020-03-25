namespace Json
{
    /// <summary>
    /// Represents an object that is JSON serializable
    /// </summary>
    public interface ISerializable 
    {
        /// <summary>
        /// Turns the object into a valid JSON string
        /// </summary>
        /// <returns>The valid JSON string</returns>
        public string JsonSerialize();
    }
}
