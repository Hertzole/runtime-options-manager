#if HERTZ_SETTINGS_INPUTSYSTEM // Only used when the new input system is installed.
using System;
using Newtonsoft.Json;

namespace Hertzole.OptionsManager
{
	public class InputActionDataJsonConverter : JsonConverter<InputActionData>
	{
		public override void WriteJson(JsonWriter writer, InputActionData value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("id");
			writer.WriteValue(value.id);
			writer.WritePropertyName("bindings");
			serializer.Serialize(writer, value.bindings);
			writer.WriteEndObject();
		}

		public override InputActionData ReadJson(JsonReader reader, Type objectType, InputActionData existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new JsonSerializationException($"Unexpected token parsing {objectType}: {reader.TokenType}");
			}

			string id = null;
			InputBindingData[] bindings = null;

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
						case "bindings":
							reader.Read();
							bindings = serializer.Deserialize<InputBindingData[]>(reader);
							break;
					}
				}
				else if (reader.TokenType == JsonToken.EndObject)
				{
					return new InputActionData(id, bindings);
				}
			}

			throw new JsonSerializationException($"Unexpected end of input while parsing {objectType}");
		}
	}
}
#endif