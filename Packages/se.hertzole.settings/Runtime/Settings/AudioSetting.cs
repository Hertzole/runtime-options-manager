using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Hertzole.Settings
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Audio Setting", menuName = "Hertzole/Settings/Audio Setting")]
#endif
	public class AudioSetting : Setting<int>
	{
		[SerializeField]
		private bool hasMinValue = true;
		[SerializeField]
		private int minValue = 0;
		[SerializeField]
		private bool hasMaxValue = true;
		[SerializeField]
		private int maxValue = 100;
		[SerializeField]
		private AudioMixerGroup mixerGroup = default;
		[SerializeField]
		private string targetProperty = default;

		public bool HasMinValue { get { return hasMinValue; } set { hasMinValue = value; } }
		public bool HasMaxValue { get { return hasMaxValue; } set { hasMaxValue = value; } }

		public int MinValue { get { return minValue; } set { minValue = value; } }
		public int MaxValue { get { return maxValue; } set { maxValue = value; } }

		public AudioMixerGroup MixerGroup { get { return mixerGroup; } set { mixerGroup = value; } }
		public string TargetProperty { get { return targetProperty; } set { targetProperty = value; } }

		protected override void SetValue(int newValue)
		{
			if (hasMinValue && newValue < minValue)
			{
				newValue = minValue;
			}
			else if (hasMaxValue && newValue > maxValue)
			{
				newValue = maxValue;
			}

			if (newValue != value)
			{
				InvokeOnValueChanging(value);
				if (mixerGroup != null)
				{
					float volume = value / 100f;
					mixerGroup.audioMixer.SetFloat(targetProperty, volume <= 0 ? -80f : Mathf.Log10(volume) * 20);
				}

				value = newValue;
				
				InvokeOnValueChanged(value);
			}
		}

		protected override int TryConvertValue(object newValue)
		{
			return Convert.ToInt32(newValue);
		}
	}
}