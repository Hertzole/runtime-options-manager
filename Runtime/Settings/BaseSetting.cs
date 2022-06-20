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

		public virtual void ResetState() { }
	}
}