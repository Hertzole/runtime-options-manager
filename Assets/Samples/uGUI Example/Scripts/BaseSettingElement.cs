using Hertzole.SettingsManager;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSettingElement : MonoBehaviour
{
	[SerializeField] 
	private Text label = default;

	public void BindSetting(Setting setting)
	{
		label.text = setting.DisplayName;
		
		OnBindSetting(setting);
	}

	protected abstract void OnBindSetting(Setting setting);
}