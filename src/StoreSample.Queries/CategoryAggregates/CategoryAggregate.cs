using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StoreSample.Queries.CategoryAggregates
{
    public class CategoryAggregate
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }

        [JsonProperty("id")]
        public Guid OriginalId { get; set; }

        public string Name { get; set; }

        public int ProductCount { get; set; }
    }
}
