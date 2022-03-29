#if HERTZ_SETTINGS_INPUTSYSTEM
using System;
using Newtonsoft.Json;

namespace Hertzole.Settings
{
	public struct InputActionData
	{
		public readonly string id;
		public readonly InputBindingData[] bindings;

		public InputActionData(string id, InputBindingData[] bindings)
		{
			this.id = id;
			this.bindings = bindings;
		}
	}

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
					string propertyName = (string)reader.Value;

					if (propertyName == "id")
					{
						id = serializer.Deserialize<string>(reader);
					}
					else if (propertyName == "bindings")
					{
						bindings = serializer.Deserialize<InputBindingData[]>(reader);
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