using System.Collections.Generic;
using System.Threading.Tasks;
using BannerflowTestApp.Data.Models;
using BannerflowTestApp.Data.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BannerflowTestApp.Data.Services
{
    public class DocumentDbRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IOptions<DocumentDbOptions> Options;
        protected IMongoDatabase Db;

        public DocumentDbRepository(IOptions<DocumentDbOptions> options)
        {
            Options = options;
            Initialize();
        }

        private void Initialize()
        {
            var client = new MongoClient(Options.Value.EndPoint);
            Db = client.GetDatabase(Options.Value.Database);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Db.GetCollection<T>(Options.Value.Collection)
                .AsQueryable()
                .ToListAsync();
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await Db.GetCollection<T>(Options.Value.Collection).Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<T> CreateAsync(T item)
        {
            await Db.GetCollection<T>(Options.Value.Collection).InsertOneAsync(item);
            return item;
        }

        public virtual async Task UpdateAsync(T item)
        {
            await Db.GetCollection<T>(Options.Value.Collection).ReplaceOneAsync(b => b.Id == item.Id, item);
        }

        public virtual async Task DeleteAsync(int id)
        {
            await Db.GetCollection<Banner>(Options.Value.Collection).DeleteOneAsync(c => c.Id == id);
        }
    }
}