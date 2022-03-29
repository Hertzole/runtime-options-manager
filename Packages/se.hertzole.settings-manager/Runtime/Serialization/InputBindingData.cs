#if HERTZ_SETTINGS_INPUTSYSTEM
using System;
using Newtonsoft.Json;

namespace Hertzole.Settings
{
	public struct InputBindingData
	{
		public readonly string id;
		public readonly string path;
		public readonly bool isOverwritten;

		public InputBindingData(string id, string path, bool isOverwritten)
		{
			this.id = id;
			this.path = path;
			this.isOverwritten = isOverwritten;
		}
	}
	
	public class InputBindingDataJsonConverter : JsonConverter<InputBindingData>
	{
		public override void WriteJson(JsonWriter writer, InputBindingData value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("id");
			writer.WriteValue(value.id);
			writer.WritePropertyName("path");
			writer.WriteValue(value.path);
			writer.WritePropertyName("isOverwritten");
			writer.WriteValue(value.isOverwritten);
			writer.WriteEndObject();
		}

		public override InputBindingData ReadJson(JsonReader reader, Type objectType, InputBindingData existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new JsonSerializationException($"Unexpected token parsing {objectType}: {reader.TokenType}");
			}

			string id = null;
			string path = null;
			bool isOverwritten = false;

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					string propertyName = (string)reader.Value;

					if (propertyName == "id")
					{
						id = serializer.Deserialize<string>(reader);
					}
					else if (propertyName == "path")
					{
						path = serializer.Deserialize<string>(reader);
					}
					else if (propertyName == "isOverwritten")
					{
						isOverwritten = serializer.Deserialize<bool>(reader);
					}
				}
				else if (reader.TokenType == JsonToken.EndObject)
				{
					return new InputBindingData(id, path, isOverwritten);
				}
			}

			throw new JsonSerializationException($"Unexpected end of input while parsing {objectType}");
		}
	}
}
#endif