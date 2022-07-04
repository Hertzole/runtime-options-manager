using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Audio Setting", menuName = "Hertzole/Settings/Audio Setting")]
#endif
	public class AudioSetting : Setting<int>, IMinMaxInt, ICanHaveSlider
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
		private bool enableSlider = true;
		[SerializeField]
		private AudioMixer targetAudioMixer = default;
		[SerializeField]
		private string targetProperty = default;

		public AudioMixer TargetAudioMixer { get { return targetAudioMixer; } set { targetAudioMixer = value; } }
		public string TargetProperty { get { return targetProperty; } set { targetProperty = value; } }

		public bool EnableSlider { get { return enableSlider; } set { enableSlider = value; } }
		public bool WholeSliderNumbers { get { return true; } }

		public bool HasMinValue { get { return hasMinValue; } set { hasMinValue = value; } }
		public bool HasMaxValue { get { return hasMaxValue; } set { hasMaxValue = value; } }

		public int MinValue { get { return minValue; } set { minValue = value; } }
		public int MaxValue { get { return maxValue; } set { maxValue = value; } }

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

				UpdateVolume(newValue);
				value = newValue;

				InvokeOnValueChanged(value);
				InvokeOnSettingChanged();
			}
		}

		protected override int TryConvertValue(object newValue)
		{
			return Convert.ToInt32(newValue);
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			base.SetSerializedValue(newValue, serializer);
			SetSerializedValueAsyncVoid(value);
		}
		
		private async void SetSerializedValueAsyncVoid(int newValue)
		{
			// There must be a delay, otherwise it will not be updated. 
			// Why? Because Unity...
			await Task.Yield();
			UpdateVolume(newValue);
		}

		private void UpdateVolume(int newValue)
		{
			if (targetAudioMixer != null && !string.IsNullOrEmpty(targetProperty))
			{
				float volume = newValue == 0 ? 0 : newValue / 100f;
				targetAudioMixer.SetFloat(targetProperty, volume <= 0 ? -80f : Mathf.Log10(volume) * 20);
			}
		}
	}
}