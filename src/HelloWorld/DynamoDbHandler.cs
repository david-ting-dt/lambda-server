using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using HelloWorld.DbItem;
using HelloWorld.Interfaces;

namespace HelloWorld
{
    public class DynamoDbHandler : IDbHandler
    {
        private readonly IAmazonDynamoDB _dbClient;
        private readonly IDynamoDBContext _dbContext;

        public DynamoDbHandler(IAmazonDynamoDB dbClient)
        {
            _dbClient = dbClient;
        }

        public DynamoDbHandler(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            var request = new ScanRequest { TableName = "People" };
            var response = await _dbClient.ScanAsync(request);
            return response.Items.Select(LoadPerson).ToList();
        }

        private static Person LoadPerson(Dictionary<string, AttributeValue> item)
        {
            return new Person
            {
                Id = item["id"].S,
                Name = item["name"].S
            };
        }

        public async Task AddPersonAsync(string id, string name)
        {
            var newPerson = new Person
            {
                Id = id,
                Name = name
            };
            await _dbContext.SaveAsync(newPerson);
        }

        public async Task DeletePersonAsync(string id)
        {
            var personToDelete = await _dbContext.LoadAsync<Person>(id);
            if (personToDelete == null)
                throw new NullReferenceException();
            await _dbContext.DeleteAsync(personToDelete);
        }

        public async Task UpdatePersonAsync(string id, string newName)
        {
            var personToUpdate = await _dbContext.LoadAsync<Person>(id);
            if (personToUpdate == null)
                throw new NullReferenceException();
            personToUpdate.Name = newName;
            await _dbContext.SaveAsync(personToUpdate);
        }
    }
}