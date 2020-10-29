using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GameStore.DAL.Serializers
{
    public class CustomStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.Int32:
                    return context.Reader.ReadInt32().ToString();
                case BsonType.String:
                    {
                        var checkingString = context.Reader.ReadString();

                        if (checkingString == "NULL")
                        {
                            return null;
                        }

                        return checkingString;
                    }

                default:
                    var message = string.Format("Cannot convert a {0} to an string.", type);
                    throw new NotSupportedException(message);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (int.TryParse(value, out int id))
            {
                context.Writer.WriteInt32(id);
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }
}
