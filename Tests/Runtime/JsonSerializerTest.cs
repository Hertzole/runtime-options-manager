using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class JsonSerializerTest : SerializerTest<JsonSettingSerializer>
	{
		[Test]
		public void DeserializeInvalidJson()
		{
			const string json = "{\"key\":\"value";

			byte[] bytes = Encoding.UTF8.GetBytes(json);
			Dictionary<string, object> data = new Dictionary<string, object>();
			LogAssert.Expect(LogType.Error, "Failed to deserialize JSON data: Unterminated string. Expected delimiter: \". Path 'key', line 1, position 13.");
			serializer.Deserialize(bytes, data);

			Assert.AreEqual(0, data.Count);
		}

		[Test]
		public void SerializeByteData()
		{
			List<string> list = new List<string>
			{
				"Hello",
				"World",
				"and",
				"hi",
				"mom!"
			};

			string json = JsonConvert.SerializeObject(list);
			byte[] byteData = Encoding.UTF8.GetBytes(json);

			List<string> newList = serializer.DeserializeType<List<string>>(byteData);
			Assert.AreEqual(list.Count, newList.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual(list[i], newList[i]);
			}
		}

#if HERTZ_SETTINGS_INPUTSYSTEM
		[Test]
		public void DeserializeInvalidInputBindingData()
		{
			const string json = "[\"id\":\"value\",\"path\":\"value\",\"isOverwritten\":true]";

			LogAssert.Expect(LogType.Error, $"Failed to deserialize JSON type: Unexpected token parsing {typeof(InputBindingData).FullName}: StartArray");
			serializer.DeserializeType<InputBindingData>(json);
		}

		[Test]
		public void DeserializeInvalidInputActionData()
		{
			const string json = "[\"id\":\"value\"]";

			LogAssert.Expect(LogType.Error, $"Failed to deserialize JSON type: Unexpected token parsing {typeof(InputActionData).FullName}: StartArray");
			serializer.DeserializeType<InputActionData>(json);
		}
#endif
	}
}