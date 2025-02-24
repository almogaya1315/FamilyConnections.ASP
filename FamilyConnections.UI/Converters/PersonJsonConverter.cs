using FamilyConnections.Core.Enums;
using FamilyConnections.UI.Models;
using Newtonsoft.Json;

namespace FamilyConnections.UI.Converters
{
    public class PersonJsonConverter : JsonConverter<Dictionary<PersonViewModel, eRel>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<PersonViewModel, eRel> value, JsonSerializer serializer)
        {
            var dict = value.ToDictionary(k => JsonConvert.SerializeObject(k.Key), v => v.Value);
            serializer.Serialize(writer, dict);
        }

        public override Dictionary<PersonViewModel, eRel> ReadJson(JsonReader reader, Type objectType, Dictionary<PersonViewModel, eRel> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<string, eRel>>(reader);
            return dict.ToDictionary(k => JsonConvert.DeserializeObject<PersonViewModel>(k.Key), v => v.Value);
        }
    }

    //public class PersonViewModelDictionaryConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(Dictionary<PersonViewModel, eRel>);
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var dictionary = (Dictionary<PersonViewModel, eRel>)value;

    //        var convertedDict = dictionary.ToDictionary(
    //            k => JsonConvert.SerializeObject(k.Key),  // Convert PersonViewModel to JSON string key
    //            v => v.Value
    //        );

    //        serializer.Serialize(writer, convertedDict);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        var dict = serializer.Deserialize<Dictionary<string, eRel>>(reader);

    //        var convertedDict = dict.ToDictionary(
    //            k => JsonConvert.DeserializeObject<PersonViewModel>(k.Key),  // Deserialize JSON key back to PersonViewModel
    //            v => v.Value
    //        );

    //        return convertedDict;
    //    }
    //}
}
