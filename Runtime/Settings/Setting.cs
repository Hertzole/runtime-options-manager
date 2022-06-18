using System;
using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Hertzole.OptionsManager
{
	public abstract class Setting : ScriptableObject
	{
#if UNITY_EDITOR
		[Header("General Settings")]
#endif
		[SerializeField]
		private string displayName = "New Setting";
#if HERTZ_SETTINGS_LOCALIZATION
		[SerializeField]
		private LocalizedString displayNameLocalized = default;
#endif
		[SerializeField]
		private string identifier = "new_setting";

#if UNITY_EDITOR
		[Header("UI")]
#endif
		[SerializeField]
		protected GameObject uiPrefab = default;
#if HERTZ_SETTINGS_UIELEMENTS
		[SerializeField]
		protected VisualTreeAsset uiElement = default;
#endif
		
#if UNITY_EDITOR
		[Header("Saving")]
#endif
		[SerializeField] 
		private bool overwriteSavePath = false;
		[SerializeField] 
		private string overriddenSavePath = default;
		[SerializeField] 
		private bool overwriteFileName = false;
		[SerializeField] 
		private string overriddenFileName = default;

		public string DisplayName { get { return displayName; } set { displayName = value; } }
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
		public string Identifier { get { return identifier; } set { identifier = value; } }
		
		public GameObject UiPrefab { get { return uiPrefab; } set { uiPrefab = value; } }

		public bool OverwriteSavePath { get { return overwriteSavePath; } set { overwriteSavePath = value; } }
		public string OverriddenSavePath { get { return overriddenSavePath; } set { overriddenSavePath = value; } }
		public bool OverwriteFileName { get { return overwriteFileName; } set { overwriteFileName = value; } }
		public string OverriddenFileName { get { return overriddenFileName; } set { overriddenFileName = value; } }

		public virtual bool CanSave { get { return true; } }

		public event Action OnSettingChanged;

		public abstract object GetDefaultValue();

		public abstract void SetSerializedValue(object newValue, ISettingSerializer serializer);

		public virtual object GetSerializeValue()
		{
			return null;
		}
		
		protected void InvokeOnSettingChanged()
		{
			OnSettingChanged?.Invoke();
		}

		public virtual GameObject CreateUIObject(Setting targetSetting, Transform parent)
		{
			return Instantiate(uiPrefab, parent);
		}

		public virtual void ResetState() { }

#if HERTZ_SETTINGS_UIELEMENTS
		public virtual VisualElement CreateUIElement()
		{
			return uiElement == null ? null : uiElement.CloneTree();
		}
#endif
	}
}