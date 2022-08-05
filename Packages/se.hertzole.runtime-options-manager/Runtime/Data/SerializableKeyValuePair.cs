using System;
using System.Collections.Generic;

namespace Hertzole.OptionsManager
{
	[Serializable]
	public struct SerializableKeyValuePair<TKey, TValue> : IEquatable<SerializableKeyValuePair<TKey, TValue>>
	{
		public TKey key;
		public TValue value;

		public SerializableKeyValuePair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public bool Equals(SerializableKeyValuePair<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(key, other.key) && EqualityComparer<TValue>.Default.Equals(value, other.value);
		}

		public override bool Equals(object obj)
		{
			return obj is SerializableKeyValuePair<TKey, TValue> other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TKey>.Default.GetHashCode(key) * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(value);
			}
		}

		public static bool operator ==(SerializableKeyValuePair<TKey, TValue> left, SerializableKeyValuePair<TKey, TValue> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SerializableKeyValuePair<TKey, TValue> left, SerializableKeyValuePair<TKey, TValue> right)
		{
			return !left.Equals(right);
		}
	}
}