using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hertzole.Settings
{
	public class SettingSerializer : ISettingSerializer
	{
		public void FillData(SettingsObject settings, Dictionary<string, object> dataToFill, bool clearData = true)
		{
			if (clearData)
			{
				dataToFill.Clear();
			}

			for (int i = 0; i < settings.Categories.Length; i++)
			{
				for (int j = 0; j < settings.Categories[i].Settings.Length; j++)
				{
					if (!settings.Categories[i].Settings[j].CanSave)
					{
						continue;
					}
					
					dataToFill.Add(settings.Categories[i].Settings[j].Identifier, settings.Categories[i].Settings[j].GetSerializeValue());
				}
			}
		}

		public string SerializeToJson(Dictionary<string, object> data, Formatting formatting = Formatting.None)
		{
			return JsonConvert.SerializeObject(data, formatting);
		}

		public void DeserializeFromJson(string json, Dictionary<string, object> dataToFill, bool clearData = true)
		{
			if (clearData)
			{
				dataToFill.Clear();
			}

			Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
			if (data == null)
			{
				return;
			}

			foreach (KeyValuePair<string, object> keyValuePair in data)
			{
				dataToFill.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}
}