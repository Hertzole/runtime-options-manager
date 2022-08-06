using System;
using System.Collections;
using Hertzole.OptionsManager.Extensions;
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
		private ToggleableInt minValue = new ToggleableInt(true, 0);
		[SerializeField]
		private ToggleableInt maxValue = new ToggleableInt(true, 100);
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

		public ToggleableInt MinValue { get { return minValue; } set { minValue = value; } }
		public ToggleableInt MaxValue { get { return maxValue; } set { maxValue = value; } }

		protected override void SetValue(int newValue)
		{
			newValue = this.Clamp(newValue);

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
			SettingsManager.StartCoroutine(SetSerializedValueRoutine(Value));
		}

		private IEnumerator SetSerializedValueRoutine(int newValue)
		{
			// There must be a delay, otherwise it will not be updated. 
			// Why? Because Unity...
			yield return null;
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