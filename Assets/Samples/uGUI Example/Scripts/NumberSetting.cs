using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Hertzole.RuntimeOptionsManager.Samples.UI
{
	public class NumberSetting : BaseSettingElement
	{
		[SerializeField]
		private Slider slider = default;
		[SerializeField]
		private Text sliderText = default;

		// Cache these to avoid closure allocations.
		private Setting<float> floatSetting;
		private Setting<int> intSetting;

		protected override void OnBindSetting(Setting setting)
		{
			if (setting is ICanHaveSlider canSlide)
			{
				slider.gameObject.SetActive(canSlide.EnableSlider);
				slider.wholeNumbers = canSlide.WholeSliderNumbers;
				slider.onValueChanged.RemoveAllListeners();
			}
			else
			{
				slider.gameObject.SetActive(false);
			}

			if (sliderText != null)
			{
				slider.onValueChanged.AddListener(x => { sliderText.text = x.ToString(CultureInfo.InvariantCulture); });
			}

			if (setting is IMinMaxFloat minMaxFloat)
			{
				slider.minValue = minMaxFloat.MinValue;
				slider.maxValue = minMaxFloat.MaxValue;
			}
			else if (setting is IMinMaxInt minMaxInt)
			{
				slider.minValue = minMaxInt.MinValue;
				slider.maxValue = minMaxInt.MaxValue;
			}

			if (setting is Setting<float> newFloatSetting)
			{
				floatSetting = newFloatSetting;
				
				slider.SetValueWithoutNotify(floatSetting.Value);
				if (sliderText != null)
				{
					sliderText.text = floatSetting.Value.ToString(CultureInfo.InvariantCulture);
				}

				slider.onValueChanged.AddListener(x => { floatSetting.Value = x; });
			}
			else if (setting is Setting<int> newIntSetting)
			{
				intSetting = newIntSetting;
				
				slider.SetValueWithoutNotify(intSetting.Value);
				if (sliderText != null)
				{
					sliderText.text = intSetting.Value.ToString();
				}

				slider.onValueChanged.AddListener(x => { intSetting.Value = (int) x; });
			}
		}
	}
}