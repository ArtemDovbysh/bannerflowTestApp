using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BannerflowTestApp.Data.Models
{
    public class Banner: BaseEntity
    {
        [BsonElement("html")]
        public string Html { get; set; }
        [BsonElement("created")]
        public DateTime Created { get; set; }
        [BsonElement("modified")]
        public DateTime? Modified { get; set; }
    }
}