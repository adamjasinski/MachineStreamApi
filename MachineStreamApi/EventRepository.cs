using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using MachineStreamApi.Models;
using MongoDB.Driver;

namespace MachineStreamApi
{
    public class EventRepository
    {
        private readonly MongoClient _client;

        public EventRepository(string mongoDbConnectionString)
        {
            if (mongoDbConnectionString == null)
                throw new ArgumentNullException(nameof(mongoDbConnectionString));
            _client = CreateClient(mongoDbConnectionString);
        }
     
        private MongoClient CreateClient(string connectionString)
        {
            var settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
            settings.SslSettings =
            new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            return mongoClient;
        }

        private IMongoCollection<MachineStreamEventData> GetCollection()
        {
            var database = _client.GetDatabase("Test");
            var collection = database.GetCollection<MachineStreamEventData>("EventData");
            return collection;
        }

        public void AddEventDataRecord(MachineStreamEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));
            var collection = GetCollection();
            collection.InsertOne(eventData);
        }

        /// <summary>
        /// Retrieves the latest status for each machine
        /// </summary>
        /// <returns></returns>
        public async Task<IList<MachineStreamEventData>> GetLatestEventsAsync()
        {
            var collection = GetCollection();
            //Apparently MongoDB .NET driver doesn't implement LINQ operators when {document} (rather than BSON) is used...
            //var res = collection
            //    .Aggregate()
            //    .Group(x => x.Machine_Id, g => g.OrderByDescending(x => x.Timestamp).First());
            //return await res.ToListAsync();
            var res = collection
                .Aggregate()
                .Group(x => x.Machine_Id,
                    g => new MachineStreamEventData { Machine_Id = g.Key, Status = g.Last().Status, Timestamp = g.Last().Timestamp});
            return await res.ToListAsync();
        }

        /// <summary>
        /// Retrieves the latest {limit} events for a given machine
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IList<MachineStreamEventData>> GetLatestEventsForMachineAsync(Guid machineId, int limit)
        {
            var collection = GetCollection();
            var res = collection
                .Find(x => x.Machine_Id == machineId)
                .Sort(
                    new SortDefinitionBuilder<MachineStreamEventData>()
                        .Descending(x => x.Timestamp))
                .Limit(limit);
            return await res.ToListAsync();
        }
    }

}
