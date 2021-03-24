﻿using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace Data.Model.Storage.MongoDB
{
    public class DynamicSerializer : SerializerBase<dynamic>
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public DynamicSerializer()
        {
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
                OutputMode = JsonOutputMode.Strict,
                Indent = true,
                GuidRepresentation = GuidRepresentation.Standard
            };
            var value = document.ToJson(writerSettings);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(value, _jsonSerializerSettings);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, dynamic value)
        {
            var o = value ?? new { };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o, _jsonSerializerSettings);
            BsonDocument document = BsonDocument.Parse(json);
            BsonDocumentSerializer.Instance.Serialize(context, document);
        }
    }
}