using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace Data.Model.Storage
{
    public class DynamicSerializer : SerializerBase<dynamic>
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        public DynamicSerializer()
        {
            //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
                FloatParseHandling = FloatParseHandling.Double
            };
        }

        public override dynamic Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var document = BsonDocumentSerializer.Instance.Deserialize(context);
            var writerSettings = new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.CanonicalExtendedJson,
                Indent = true
            };
            var value = document.ToJson(writerSettings);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(value, _jsonSerializerSettings);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, dynamic value)
        {
            var item = value ?? new { };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(item, _jsonSerializerSettings);
            BsonDocument document = BsonDocument.Parse(json);
            BsonDocumentSerializer.Instance.Serialize(context, document);
        }
    }
}