using System.Globalization;
using Hertzole.SettingsManager;
using UnityEngine;
using UnityEngine.UI;

public class NumberSetting : BaseSettingElement
{
	[SerializeField]
	private Slider slider = default;
	[SerializeField]
	private Text sliderText = default;

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

		if (setting is Setting<float> floatSetting)
		{
			slider.SetValueWithoutNotify(floatSetting.Value);
			if (sliderText != null)
			{
				sliderText.text = floatSetting.Value.ToString(CultureInfo.InvariantCulture);
			}

			slider.onValueChanged.AddListener(x => { floatSetting.Value = x; });
		}
		else if (setting is Setting<int> intSetting)
		{
			slider.SetValueWithoutNotify(intSetting.Value);
			if (sliderText != null)
			{
				sliderText.text = intSetting.Value.ToString();
			}

			slider.onValueChanged.AddListener(x => { intSetting.Value = (int) x; });
		}
	}
}