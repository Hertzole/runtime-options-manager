using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Hertzole.OptionsManager
{
	public partial class SettingsManager
	{
		internal class SettingsManagerBehavior : MonoBehaviour
		{
			private bool dirtySave;
			private bool startedListeningForSave;

			private readonly Dictionary<string, object> settingData = new Dictionary<string, object>();
			private readonly HashSet<string> loadedSettings = new HashSet<string>();

			private float saveTime;
			private readonly HashSet<string> excludedSettings = new HashSet<string>();
			private SettingsManager manager;

			private static readonly StringBuilder pathBuilder = new StringBuilder();

			public SettingsManager Manager
			{
				get { return manager; }
				internal set
				{
					manager = value;
					if (startedListeningForSave)
					{
						ToggleSettingsListening(false);
					}

					ToggleSettingsListening(true);
				}
			}
			public ISettingSerializer Serializer { get; internal set; }

			public bool AutoSaveSettings { get; internal set; }

			public string SavePath { get; internal set; }
			public bool HasLoadedSettings { get; private set; }

			private void Update()
			{
				if (AutoSaveSettings && dirtySave && Time.unscaledTime >= saveTime)
				{
					SaveSettings();
				}
			}

			private void OnDestroy()
			{
				if (AutoSaveSettings && dirtySave)
				{
					SaveSettings();
				}
			}

			private void ToggleSettingsListening(bool listen)
			{
				foreach (SettingsCategory category in manager.categories)
				{
					foreach (BaseSetting baseSetting in category.Settings)
					{
						if (baseSetting is Setting setting)
						{
							if (listen)
							{
								setting.OnSettingChanged += OnSettingChanged;
							}
							else
							{
								setting.OnSettingChanged -= OnSettingChanged;
							}
						}
					}
				}

				startedListeningForSave = listen;
			}

			private void OnSettingChanged()
			{
				dirtySave = true;
				saveTime = Time.unscaledTime + 1f;
			}

			private readonly Dictionary<string, List<Setting>> settingPaths = new Dictionary<string, List<Setting>>();

			internal void SaveSettings()
			{
				dirtySave = false;

				GetSavePaths();

				foreach (KeyValuePair<string,List<Setting>> settingPath in settingPaths)
				{
					string directory = Path.GetDirectoryName(settingPath.Key);

					if (!Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory!);
					}
					
					settingData.Clear();
					GetSerializeData(settingData, settingPath.Value);
					
					byte[] data = Serializer.Serialize(settingData);
					File.WriteAllBytes(settingPath.Key, data);
				}
			}

			private bool isLoadingSettings;

			internal void LoadSettings()
			{
				if (isLoadingSettings)
				{
					return;
				}

				HasLoadedSettings = false;
				
				settingData.Clear();
				excludedSettings.Clear();
				loadedSettings.Clear();
				
				if (!File.Exists(SavePath))
				{
					SetDefaultValues(excludedSettings);
					HasLoadedSettings = true;
					return;
				}

				isLoadingSettings = true;

				StartCoroutine(LoadSettingsRoutine());
			}

			private IEnumerator LoadSettingsRoutine()
			{
#if HERTZ_SETTINGS_LOCALIZATION
				if (LocalizationSettings.Instance.GetAvailableLocales() is LocalesProvider localesProvider)
				{
					var loadOperation = localesProvider.PreloadOperation;
					while(!loadOperation.IsDone)
					{
						yield return null;
					}
				}
#endif

				GetSavePaths();

				foreach (string settingPath in settingPaths.Keys)
				{
					if(!File.Exists(settingPath))
					{
						continue;
					}

					byte[] data = File.ReadAllBytes(settingPath);
					Serializer.Deserialize(data, settingData);
					Dictionary<string, object>.Enumerator enumerator = settingData.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, object> current = enumerator.Current;

						if (Manager.TryGetSetting(current.Key, out Setting setting))
						{
							loadedSettings.Add(current.Key);
							setting.SetSerializedValue(current.Value, Serializer);
							excludedSettings.Add(current.Key);
						}
					}

					enumerator.Dispose();
				}
				
				SetDefaultValues(excludedSettings);
				isLoadingSettings = false;
				HasLoadedSettings = true;

				yield return null;
			}

			internal bool HasLoadedSetting(string identifier)
			{
				return loadedSettings.Contains(identifier);
			}

			private void GetSavePaths()
			{
				settingPaths.Clear();
				
				foreach (SettingsCategory category in Manager.categories)
				{
					foreach (BaseSetting baseSetting in category.Settings)
					{
						if (!(baseSetting is Setting setting) || !setting.CanSave)
						{
							continue;
						}

						string savePath = GetSettingSavePath(setting);
						if (!settingPaths.TryGetValue(savePath, out List<Setting> list))
						{
							list = new List<Setting>(1);
							settingPaths.Add(savePath, list);
						}

						list.Add(setting);
					}
				}
			}

			internal static void GetSerializeData(Dictionary<string, object> dataBuffer, IEnumerable<Setting> settings)
			{
				foreach (Setting setting in settings)
				{
					dataBuffer[setting.Identifier] = setting.GetSerializeValue();
				}
			}

			internal void SetDefaultValues(ICollection<string> excluding = null)
			{
				foreach (SettingsCategory category in manager.categories)
				{
					foreach (BaseSetting baseSetting in category.Settings)
					{
						if (baseSetting is Setting setting)
						{
							if(excluding == null || !excluding.Contains(setting.Identifier))
							{
								setting.SetSerializedValue(setting.GetDefaultValue(), Serializer);
							}
						}
					}
				}
			}

			private string GetSettingSavePath(Setting setting)
			{
				if (!setting.OverwriteSavePath && !setting.OverwriteFileName)
				{
					return SavePath;
				}
				
				pathBuilder.Clear();

				pathBuilder.Append(GetSaveLocation(manager.saveLocation, manager.customPathProvider));
				pathBuilder.Append('/');

				pathBuilder.Append(setting.OverwriteSavePath ? setting.OverriddenSavePath : manager.savePath);
				pathBuilder.Append('/');
				pathBuilder.Append(setting.OverwriteFileName ? setting.OverriddenFileName : manager.fileName);

				return Path.GetFullPath(pathBuilder.ToString());
			}
		}
	}
}