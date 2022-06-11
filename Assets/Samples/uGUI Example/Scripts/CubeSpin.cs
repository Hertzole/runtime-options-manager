using UnityEngine;

namespace Hertzole.SettingsManager.Samples.UI
{
	public class CubeSpin : MonoBehaviour
	{
		public bool Spin { get; set; } = true;

		public float SpinSpeed { get; set; } = 90;

		private void Update()
		{
			if (Spin)
			{
				transform.Rotate(Vector3.up * SpinSpeed * Time.deltaTime);
			}
		}
	}
}