using System;
using System.Linq;
using System.Threading.Tasks;
using BannerflowTestApp.Data.Models;
using BannerflowTestApp.Data.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BannerflowTestApp.Data.Services
{
    public class BannersRepository : DocumentDbRepository<Banner>
    {
        public BannersRepository(IOptions<DocumentDbOptions> options) : base(options)
        {
        }

        public override async Task<Banner> CreateAsync(Banner banner)
        {
            HtmlValidation(banner.Html);

            banner.Created = DateTime.UtcNow;
            return await base.CreateAsync(banner);
        }
        
        public override async Task UpdateAsync(Banner banner)
        {
            HtmlValidation(banner.Html);
            
            var filter = Builders<Banner>.Filter.Eq(b => b.Id, banner.Id);
            var update = Builders<Banner>.Update.Set(b => b.Html, banner.Html)
                                                .Set(b => b.Modified, DateTime.UtcNow);
            await Db.GetCollection<Banner>(Options.Value.Collection).UpdateOneAsync(filter, update);
        }

        private static void HtmlValidation(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            if (doc.ParseErrors.Any())
            {
                throw new ArgumentException("html");
            }
        }
    }
}
