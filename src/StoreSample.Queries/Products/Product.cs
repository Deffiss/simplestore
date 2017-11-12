using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSample.Queries.Products
{
    public class Product
    {
        [BsonId, JsonIgnore]
        public ObjectId Id { get; set; }

        [JsonProperty("id")]
        public Guid OriginalId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CategoryId { get; set; }

        public string ImagePath { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
