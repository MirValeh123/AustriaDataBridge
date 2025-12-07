using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Models.Response
{
    public class ExportJobLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string JobNumber { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public string EquipmentType { get; set; } = string.Empty;
        public int CardAmount { get; set; }
    }
}
