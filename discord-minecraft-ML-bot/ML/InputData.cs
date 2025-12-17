using Microsoft.ML.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace Bot
{
    public class InputData
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id {get; set;}

        
        [BsonElement("col1")]
        public bool Label { get; set; }
        [BsonElement("col2")]
        public string? Message { get; set; }
    }
}