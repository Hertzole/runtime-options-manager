using System;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	[Serializable]
	public struct ToggleableFloat : IEquatable<ToggleableFloat>
	{
		[SerializeField] 
		private bool enabled;
		[SerializeField] 
		private float value;

		public bool Enabled { get { return enabled; } }
		public float Value { get { return value; } }

		public ToggleableFloat(bool enabled, float value)
		{
			this.enabled = enabled;
			this.value = value;
		}

		public bool Equals(ToggleableFloat other)
		{
			return enabled == other.enabled && value.Equals(other.value);
		}

		public override bool Equals(object obj)
		{
			return obj is ToggleableFloat other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (enabled.GetHashCode() * 397) ^ value.GetHashCode();
			}
		}

		public static bool operator ==(ToggleableFloat left, ToggleableFloat right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ToggleableFloat left, ToggleableFloat right)
		{
			return !left.Equals(right);
		}
		
		public static implicit operator float(ToggleableFloat toggleableFloat)
		{
			return toggleableFloat.Value;
		}
		
		public static implicit operator ToggleableFloat(float value)
		{
			return new ToggleableFloat(true, value);
		}
		
		public static implicit operator ToggleableFloat(bool enabled)
		{
			return new ToggleableFloat(enabled, 0);
		}
	}
}