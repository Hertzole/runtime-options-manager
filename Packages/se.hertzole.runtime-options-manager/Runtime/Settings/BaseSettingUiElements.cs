#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.OptionsManager
{
	public partial class BaseSetting
	{
		[SerializeField]
		protected VisualTreeAsset uiElement = default;

		public VisualTreeAsset UIElement { get { return uiElement; } set { uiElement = value; } }
	}
}
#endif