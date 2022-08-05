using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class SerializableKeyValuePairTests
	{
		[Test]
		public void HashCodeMatch_True()
		{
			SerializableKeyValuePair<int, int> pair1 = new SerializableKeyValuePair<int, int>(42, 69);
			SerializableKeyValuePair<int, int> pair2 = new SerializableKeyValuePair<int, int>(42, 69);

			Assert.AreEqual(pair1.GetHashCode(), pair2.GetHashCode());
		}
		
		[Test]
		public void HashCodeMatch_False()
		{
			SerializableKeyValuePair<int, int> pair1 = new SerializableKeyValuePair<int, int>(42, 69);
			SerializableKeyValuePair<int, int> pair2 = new SerializableKeyValuePair<int, int>(420, 69);

			Assert.AreNotEqual(pair1.GetHashCode(), pair2.GetHashCode());
		}
		
		[Test]
		public void Equals_True()
		{
			SerializableKeyValuePair<int, int> pair1 = new SerializableKeyValuePair<int, int>(42, 69);
			SerializableKeyValuePair<int, int> pair2 = new SerializableKeyValuePair<int, int>(42, 69);

			Assert.IsTrue(pair1 == pair2);
		}

		[Test]
		public void Equals_False()
		{
			SerializableKeyValuePair<int, int> pair1 = new SerializableKeyValuePair<int, int>(42, 69);
			SerializableKeyValuePair<int, int> pair2 = new SerializableKeyValuePair<int, int>(420, 69);

			Assert.IsTrue(pair1 != pair2);
		}

		[Test]
		public void Equals_Invalid()
		{
			SerializableKeyValuePair<int, int> pair1 = new SerializableKeyValuePair<int, int>(42, 69);
			object invalidType = new object();
			
			Assert.IsFalse(pair1.Equals(invalidType));
		}
	}
}