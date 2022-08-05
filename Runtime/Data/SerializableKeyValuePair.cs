using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	[Serializable]
	public struct SerializableKeyValuePair<TKey, TValue> : IEquatable<SerializableKeyValuePair<TKey, TValue>>
	{
		[SerializeField]
		private TKey key;
		[SerializeField]
		private TValue value;

		public TKey Key { get { return key; } }
		public TValue Value { get { return value; } }

		public SerializableKeyValuePair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TKey>.Default.GetHashCode(Key) * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value);
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

		public override bool Equals(object obj)
		{
			return obj is SerializableKeyValuePair<TKey, TValue> other && Equals(other);
		}

		public bool Equals(SerializableKeyValuePair<TKey, TValue> other)
		{
			return EqualityComparer<TKey>.Default.Equals(key, other.key) && EqualityComparer<TValue>.Default.Equals(value, other.value);
		}
	}
}