using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hertzole.Settings
{
	public interface ISettingSerializer
	{
		void FillData(SettingsObject settings, Dictionary<string, object> dataToFill, bool clearData = true);
		
		string SerializeToJson(Dictionary<string, object> data, Formatting formatting = Formatting.None);

		void DeserializeFromJson(string json, Dictionary<string, object> dataToFill, bool clearData = true);
	}
}