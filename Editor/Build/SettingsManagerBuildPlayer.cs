using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Hertzole.OptionsManager.Editor
{
	// Mostly stolen from Unity's localization package.
	public class SettingsManagerBuildPlayer : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		private bool removeFromPreloadedAssets;

		private SettingsManager settingsManager;

		public int callbackOrder { get { return 1; } }

		public void OnPreprocessBuild(BuildReport report)
		{
			removeFromPreloadedAssets = false;
			settingsManager = SettingsManagerEditorSettings.ActiveSettings;
			if (settingsManager == null)
			{
				return;
			}

			Object[] preloadedAsset = PlayerSettings.GetPreloadedAssets();
			bool wasDirty = IsPlayerSettingsDirty();

			if (!preloadedAsset.Contains(settingsManager))
			{
				ArrayUtility.Add(ref preloadedAsset, settingsManager);
				PlayerSettings.SetPreloadedAssets(preloadedAsset);

				removeFromPreloadedAssets = true;

				if (!wasDirty)
				{
					ClearPlayerSettingsDirtyFlag();
				}
			}
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			if (settingsManager == null || !removeFromPreloadedAssets)
			{
				return;
			}

			bool wasDirty = IsPlayerSettingsDirty();

			Object[] preloadedAsset = PlayerSettings.GetPreloadedAssets();
			ArrayUtility.Remove(ref preloadedAsset, settingsManager);
			PlayerSettings.SetPreloadedAssets(preloadedAsset);

			settingsManager = null;

			if (!wasDirty)
			{
				ClearPlayerSettingsDirtyFlag();
			}
		}

		private static bool IsPlayerSettingsDirty()
		{
			PlayerSettings[] settings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (settings != null && settings.Length > 0)
			{
				return EditorUtility.IsDirty(settings[0]);
			}

			return false;
		}

		private static void ClearPlayerSettingsDirtyFlag()
		{
			PlayerSettings[] settings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (settings != null && settings.Length > 0)
			{
				EditorUtility.ClearDirty(settings[0]);
			}
		}
	}
}