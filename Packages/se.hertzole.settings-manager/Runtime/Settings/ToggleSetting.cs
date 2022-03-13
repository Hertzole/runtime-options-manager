using System;
using UnityEngine;

namespace Hertzole.Settings
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Toggle Setting", menuName = "Hertzole/Settings/Toggle Setting")]
#endif
	public class ToggleSetting : Setting<bool>
	{
		protected override bool TryConvertValue(object newValue)
		{
			return Convert.ToBoolean(newValue);
		}
	}
}