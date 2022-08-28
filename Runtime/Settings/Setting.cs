using System;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	public abstract class Setting : BaseSetting
	{
		[SerializeField]
		private string identifier = "new_setting";

		[SerializeField]
		private bool overwriteSavePath = false;
		[SerializeField]
		private string overriddenSavePath = default;
		[SerializeField]
		private bool overwriteFileName = false;
		[SerializeField]
		private string overriddenFileName = default;

		public string Identifier { get { return identifier; } set { identifier = value; } }

		public bool OverwriteSavePath { get { return overwriteSavePath; } set { overwriteSavePath = value; } }
		public string OverriddenSavePath { get { return overriddenSavePath; } set { overriddenSavePath = value; } }
		public bool OverwriteFileName { get { return overwriteFileName; } set { overwriteFileName = value; } }
		public string OverriddenFileName { get { return overriddenFileName; } set { overriddenFileName = value; } }

		public virtual bool CanSave { get { return true; } }

		/// <summary> If true, it won't invoke OnSettingChanged when the value is changed. </summary>
		protected bool DontInvokeSettingChanged { get; set; }

		public event Action OnSettingChanged;

		public abstract object GetDefaultValue();

		public abstract void SetSerializedValue(object newValue, ISettingSerializer serializer);

		public virtual object GetSerializeValue()
		{
			return null;
		}

		protected void InvokeOnSettingChanged()
		{
			if (!DontInvokeSettingChanged)
			{
				OnSettingChanged?.Invoke();
			}
		}
	}
}