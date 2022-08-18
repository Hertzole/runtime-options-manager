using System;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	[Serializable]
	public struct ToggleableInt : IEquatable<ToggleableInt>
	{
		[SerializeField] 
		private bool enabled;
		[SerializeField] 
		private int value;

		public bool Enabled { get { return enabled; } }
		public int Value { get { return value; } }

		public ToggleableInt(bool enabled, int value)
		{
			this.enabled = enabled;
			this.value = value;
		}

		public bool Equals(ToggleableInt other)
		{
			return enabled == other.enabled && value.Equals(other.value);
		}

		public override bool Equals(object obj)
		{
			return obj is ToggleableInt other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Enabled.GetHashCode() * 397) ^ Value.GetHashCode();
			}
		}

		public static bool operator ==(ToggleableInt left, ToggleableInt right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ToggleableInt left, ToggleableInt right)
		{
			return !left.Equals(right);
		}
		
		public static implicit operator int(ToggleableInt toggleableFloat)
		{
			return toggleableFloat.Value;
		}
		
		public static implicit operator ToggleableInt(int value)
		{
			return new ToggleableInt(true, value);
		}
		
		public static implicit operator ToggleableInt(bool enabled)
		{
			return new ToggleableInt(enabled, 0);
		}
	}
}