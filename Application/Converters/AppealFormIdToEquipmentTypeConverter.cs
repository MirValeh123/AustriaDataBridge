using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.External.Taxograf.Converters
{
    public class AppealFormIdToEquipmentTypeConverter : JsonConverter<EquipmentTypeEnum>
    {
        public override EquipmentTypeEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var value = reader.GetInt32();
                return (EquipmentTypeEnum)value;
            }
            return EquipmentTypeEnum.None;
        }

        public override void Write(Utf8JsonWriter writer, EquipmentTypeEnum value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value);
        }
    }
}