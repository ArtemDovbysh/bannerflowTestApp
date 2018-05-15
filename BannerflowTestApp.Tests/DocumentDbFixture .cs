using System;
using System.Threading.Tasks;
using BannerflowTestApp.Data.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BannerflowTestApp.Tests
{
    public class DocumentDbFixture : IDisposable
    {
        protected readonly MongoClient MongoClient;
        protected readonly IMongoDatabase Database;

        public IOptions<DocumentDbOptions> Options { get; } =
            Microsoft.Extensions.Options.Options.Create(new DocumentDbOptions
            {
                EndPoint = string.Empty,
                Database = Guid.NewGuid().ToString("N"),
                Collection = Guid.NewGuid().ToString("N"),
            });

        public DocumentDbFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Options.Value.EndPoint = config.GetSection("DocumentDb:Endpoint").Value;
            MongoClient = new MongoClient(Options.Value.EndPoint);
            Database = MongoClient.GetDatabase(Options.Value.Database);
        }

        public async Task CreateCollectionAsync()
        {
            await Database.CreateCollectionAsync(Options.Value.Collection);
        }

        public async Task DeleteCollectionAsync()
        {
            await Database.DropCollectionAsync(Options.Value.Collection);
        }

        public void Dispose()
        {
            MongoClient.DropDatabase(Options.Value.Database);
        }
    }
}