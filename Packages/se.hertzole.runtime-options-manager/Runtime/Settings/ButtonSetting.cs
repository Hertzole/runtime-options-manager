using UnityEngine;
using UnityEngine.Events;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Button Setting Decorator", menuName = "Hertzole/Settings/Decorators/Button", order = -100)]
#endif
	public class ButtonSetting : SettingDecorator
	{
		[SerializeField]
		private UnityEvent onClick = new UnityEvent();

		public UnityEvent OnClick { get { return onClick; } set { onClick = value; } }
	}
}