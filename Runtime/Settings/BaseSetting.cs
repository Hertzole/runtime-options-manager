using UnityEngine;

namespace Hertzole.OptionsManager
{
	public abstract partial class BaseSetting : ScriptableObject
	{
		[SerializeField]
		private string displayName = "New Setting";
		[SerializeField]
		protected GameObject uiPrefab = default;

		public string DisplayName { get { return displayName; } set { displayName = value; } }
		public GameObject UiPrefab { get { return uiPrefab; } set { uiPrefab = value; } }

		protected SettingsManager SettingsManager { get; private set; }

		public void Initialize(SettingsManager manager)
		{
			if (SettingsManager == manager)
			{
				return;
			}
			
			SettingsManager = manager;
			
			OnInitialize();
		}
		
		protected virtual void OnInitialize() { }

		public virtual void ResetState() { }
	}
}