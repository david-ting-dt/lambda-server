using Amazon.DynamoDBv2.DataModel;

namespace HelloWorld.DbItem
{
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