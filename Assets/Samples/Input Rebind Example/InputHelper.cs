using System;
using UnityEngine;

namespace Hertzole.OptionsManager.Samples.InputRebinding
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Input Helper", menuName = "Hertzole/Settings/Samples/Input Helper")]
#endif
	public class InputHelper : ScriptableObject
	{
		public event Action OnResetControlsRequested;
		
		public void ResetControls()
		{
			 OnResetControlsRequested?.Invoke();
		}
	}
}