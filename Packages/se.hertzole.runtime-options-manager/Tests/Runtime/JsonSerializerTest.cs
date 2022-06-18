using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hertzole.RuntimeOptionsManager.Tests
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
	}
}