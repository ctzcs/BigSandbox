using System;
using Newtonsoft.Json;


namespace ScriptsBox.存档系统
{
    [JsonConverter(typeof(AObjectConvert))]
    public class ASerializedObject
    {
        public int id;
        public string name;
    }

    class AObjectConvert:JsonConverter<ASerializedObject>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, ASerializedObject value, JsonSerializer serializer)
        {
            string str = $"{value.id},{value.name}";
            writer.WriteValue(str);
        }

        public override ASerializedObject ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, ASerializedObject existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return Parse2AObject(reader.Value as string);
        }

        public static ASerializedObject Parse2AObject(string str)
        {
            var array = str.Split(",");
            return new ASerializedObject()
            {
                id = int.Parse(array[0]),
                name = array[1]
            };
        }
    }
}