#if HERTZ_SETTINGS_INPUTSYSTEM // Only used when the new input system is installed.
using System;
using Newtonsoft.Json;

namespace Hertzole.SettingsManager
{
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
			bool? isOverwritten = false;

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					string propertyName = (string) reader.Value;

					switch (propertyName)
					{
						case "id":
							id = reader.ReadAsString();
							break;
						case "path":
							path = reader.ReadAsString();
							break;
						case "isOverwritten":
							isOverwritten = reader.ReadAsBoolean();
							break;
					}
				}
				else if (reader.TokenType == JsonToken.EndObject)
				{
					return new InputBindingData(id, path, isOverwritten ?? false);
				}
			}

			throw new JsonSerializationException($"Unexpected end of input while parsing {objectType}");
		}
	}
}
#endif