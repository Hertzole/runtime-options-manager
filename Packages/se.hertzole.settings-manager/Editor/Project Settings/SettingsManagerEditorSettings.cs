using UnityEditor;
using UnityEngine;

namespace Hertzole.Settings.Editor
{
	public class SettingsManagerEditorSettings
	{
		private static SettingsManagerEditorSettings instance;

		internal static SettingsManagerEditorSettings Instance { get { return instance ??= new SettingsManagerEditorSettings(); } }

		public static SettingsManager ActiveSettings { get { return Instance.ActiveSettingsInternal; } set { Instance.ActiveSettingsInternal = value; } }

		internal SettingsManager ActiveSettingsInternal
		{
			get
			{
				EditorBuildSettings.TryGetConfigObject(SettingsManager.CONFIG_NAME, out SettingsManager settings);
				return settings;
			}
			set
			{
				if (value == null)
				{
					EditorBuildSettings.RemoveConfigObject(SettingsManager.CONFIG_NAME);
				}
				else
				{
					EditorBuildSettings.AddConfigObject(SettingsManager.CONFIG_NAME, value, true);
				}
			}
		}

		public static SettingsManager CreateSettings()
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Settings Manager", "Settings Manager", "asset", "Please enter a filename to save the projects settings manager to.");

			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			SettingsManager settings = ScriptableObject.CreateInstance<SettingsManager>();
			settings.name = "Settings Manager";

			AssetDatabase.CreateAsset(settings, path);
			AssetDatabase.SaveAssets();
			return settings;
		}
	}
}