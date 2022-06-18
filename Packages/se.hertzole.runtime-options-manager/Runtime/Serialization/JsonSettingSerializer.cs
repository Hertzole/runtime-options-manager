using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Hertzole.SettingsManager
{
	[Serializable]
	public class JsonSettingSerializer : ISettingSerializer
	{
		[SerializeField] 
		private bool prettyPrint = false;

		private readonly JsonSerializerSettings serializerSettings;

		public JsonSettingSerializer()
		{
			serializerSettings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter>
				{
#if HERTZ_SETTINGS_INPUTSYSTEM
					new InputActionDataJsonConverter(),
					new InputBindingDataJsonConverter()
#endif
				}
			};
		}
		
		public byte[] Serialize(Dictionary<string, object> data)
		{
			if (data == null)
			{
				return Array.Empty<byte>();
			}
			
			string json = JsonConvert.SerializeObject(data, prettyPrint ? Formatting.Indented : Formatting.None, serializerSettings);
			return System.Text.Encoding.UTF8.GetBytes(json);
		}

		public void Deserialize(byte[] data, Dictionary<string, object> dataToFill)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			
			string json = System.Text.Encoding.UTF8.GetString(data);
			Dictionary<string, object> deserializedData = null;
			try
			{
				deserializedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, serializerSettings);
			}
			catch (JsonException e)
			{
				deserializedData = null;
				Debug.LogError($"Failed to deserialize JSON data: {e.Message}");
			}

			if(deserializedData == null)
			{
				return;
			}
			
			foreach (KeyValuePair<string, object> entry in deserializedData)
			{
				dataToFill[entry.Key] = entry.Value;
			}
		}

		public T DeserializeType<T>(object data)
		{
			if (data == null)
			{
				return default;
			}
		
			string json;
			switch (data)
			{
				case JObject jObject:
					json = jObject.ToString();
					break;
				case string stringData:
					json = stringData;
					break;
				case byte[] byteData:
					json = System.Text.Encoding.UTF8.GetString(byteData);
					break;
				default:
					throw new ArgumentException("Unable to deserialize type " + typeof(T).Name + " from data of type " + data.GetType().Name);
			}
			
			return JsonConvert.DeserializeObject<T>(json, serializerSettings);
		}
	}
}