using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hertzole.OptionsManager
{
	public interface ISettingSerializer
	{
		byte[] Serialize(Dictionary<string, object> data);

		void Deserialize(byte[] data, Dictionary<string, object> dataToFill);
		
		T DeserializeType<T>(object data);
	}
}