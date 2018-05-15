using MongoDB.Bson.Serialization.Attributes;

namespace BannerflowTestApp.Data.Models
{
    public class BaseEntity
    {
        [BsonId]
        public int Id { get; set; }
    }
}