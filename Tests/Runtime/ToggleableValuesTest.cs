using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class ToggleableValuesTest : BaseTest
	{
		[Test]
		public void ToggleableFloat_Equals()
		{
			ToggleableFloat a = new ToggleableFloat(true, 1f);
			ToggleableFloat b = new ToggleableFloat(true, 1f);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a == b);

			a = new ToggleableFloat(false, 1f);

			Assert.IsFalse(a.Equals(b));
			Assert.IsTrue(a != b);

			b = new ToggleableFloat(false, 1f);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a == b);

			a = new ToggleableFloat(true, 2f);

			Assert.IsFalse(a.Equals(b));
			Assert.IsTrue(a != b);

			b = new ToggleableFloat(true, 2f);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a == b);
		}

		[Test]
		public void ToggleableInt_Equals()
		{
			ToggleableInt a = new ToggleableInt(true, 1);
			ToggleableInt b = new ToggleableInt(true, 1);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a == b);

			a = new ToggleableInt(false, 1);

			Assert.IsFalse(a.Equals(b));
			Assert.IsTrue(a != b);

			b = new ToggleableInt(false, 1);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a == b);

			a = new ToggleableInt(true, 2);

			Assert.IsFalse(a.Equals(b));
			Assert.IsTrue(a != b);

			b = new ToggleableInt(true, 2);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(a == b);
		}

		[Test]
		public void ToggleableFloat_EqualsObject()
		{
			ToggleableFloat a = new ToggleableFloat(true, 1);
			ToggleableFloat b = new ToggleableFloat(true, 1);

			Assert.IsTrue(a.Equals((object) b));

			Assert.IsFalse(a.Equals(new Vector3(0, 0, 0)));
		}

		[Test]
		public void ToggleableInt_EqualsObject()
		{
			ToggleableInt a = new ToggleableInt(true, 1);
			ToggleableInt b = new ToggleableInt(true, 1);

			Assert.IsTrue(a.Equals((object) b));

			Assert.IsFalse(a.Equals(new Vector3(0, 0, 0)));
		}
		
		[Test]
		public void ToggleableFloat_GetHashCode()
		{
			ToggleableFloat a = new ToggleableFloat(true, 1f);
			ToggleableFloat b = new ToggleableFloat(true, 1f);

			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

			a = new ToggleableFloat(false, 1f);
			
			Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

			b = new ToggleableFloat(false, 1f);
			
			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

			a = new ToggleableFloat(true, 2f);
			
			Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

			b = new ToggleableFloat(true, 2f);
			
			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		}
		
		[Test]
		public void ToggleableInt_GetHashCode()
		{
			ToggleableInt a = new ToggleableInt(true, 1);
			ToggleableInt b = new ToggleableInt(true, 1);

			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

			a = new ToggleableInt(false, 1);
			
			Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

			b = new ToggleableInt(false, 1);
			
			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

			a = new ToggleableInt(true, 2);
			
			Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());

			b = new ToggleableInt(true, 2);
			
			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		}

		[Test]
		public void ToggleableFloat_ToFloatOperator()
		{
			ToggleableFloat a = 42f;
			Assert.AreEqual(a.Value, 42f);
		}

		[Test]
		public void ToggleableFloat_ToBoolOperator([ValueSource(nameof(boolValues))] bool enabled)
		{
			ToggleableFloat a = enabled;
			Assert.AreEqual(enabled, a);
		}

		[Test]
		public void ToggleableInt_ToIntOperator()
		{
			ToggleableInt a = 42;
			Assert.AreEqual(a, 42);
		}

		[Test]
		public void ToggleableInt_ToBoolOperator([ValueSource(nameof(boolValues))] bool enabled)
		{
			ToggleableInt a = enabled;
			Assert.AreEqual(enabled, a);
		}
	}
}