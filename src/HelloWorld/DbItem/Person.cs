using Amazon.DynamoDBv2.DataModel;

namespace HelloWorld.DbItem
{
    /// <summary>
    /// Maps to the Person item in database People Table
    /// </summary>
    [DynamoDBTable("People")]
    public class Person
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }
        [DynamoDBProperty("name")]
        public string Name { get; set; }
        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}