using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization.Settings;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Settings Manager", menuName = "Hertzole/Settings/Settings Manager", order = -10000)]
#endif
	[DefaultExecutionOrder(-1000000)]
	public partial class SettingsManager : ScriptableObject
	{
		public enum SaveLocations
		{
			PersistentDataPath = 0,
			DataPath = 1,
			Documents = 2,
			Custom = 3
		}

		[SerializeField]
		private bool autoSaveSettings = true;
		[SerializeField] 
		private bool loadSettingsOnBoot = true;

		[SerializeField]
		private SaveLocations saveLocation = default;
		[SerializeField]
		private string savePath = "settings";
		[SerializeField]
		private string fileName = "settings.json";
		[SerializeReference] 
		private ISettingPathProvider customPathProvider = null;
		[SerializeReference] 
		private ISettingSerializer serializer = new JsonSettingSerializer();

		[SerializeField]
		private List<SettingsCategory> categories = new List<SettingsCategory>();

		private bool isInitialized = false;

		private readonly Dictionary<string, Setting> cachedSettings = new Dictionary<string, Setting>();

		private static SettingsManager instance;

		private SettingsManagerBehavior behavior;

		private static readonly StringBuilder savePathBuilder = new StringBuilder();

		public bool AutoSaveSettings { get { return autoSaveSettings; } set { autoSaveSettings = value; } }
		public bool LoadSettingsOnBoot { get { return loadSettingsOnBoot; } set { loadSettingsOnBoot = value; } }
		public SaveLocations SaveLocation
		{
			get { return saveLocation; }
			set
			{
				if (saveLocation != value)
				{
					saveLocation = value;
					ComputedSavePath = GetSavePath(this);
					if (behavior != null)
					{
						behavior.SavePath = ComputedSavePath;
					}
				}
			}
		}
		public string SavePath
		{
			get { return savePath; }
			set
			{
				if (savePath != value)
				{
					savePath = value;
					ComputedSavePath = GetSavePath(this);
					if (behavior != null)
					{
						behavior.SavePath = ComputedSavePath;
					}
				}
			}
		}
		public string FileName
		{
			get { return fileName; }
			set
			{
				if (fileName != value)
				{
					fileName = value;
					ComputedSavePath = GetSavePath(this);
					if (behavior != null)
					{
						behavior.SavePath = ComputedSavePath;
					}
				}
			}
		}

		public bool HasLoadedSettings { get { return behavior != null && behavior.HasLoadedSettings; } }

		public string ComputedSavePath { get; private set; }

		public ISettingPathProvider CustomPathProvider { get { return customPathProvider; } set { customPathProvider = value; } }
		public ISettingSerializer Serializer { get { return serializer; } set { serializer = value; } }
		public List<SettingsCategory> Categories { get { return categories; } set { categories = value; } }

		public static SettingsManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GetOrCreateSettings();
				}

				TryCreateBehavior();
				return instance;
			}
			set
			{
				if (instance != null && instance.behavior != null)
				{
					Destroy(instance.behavior.gameObject);
				}
				
				instance = value;
				if (instance != null)
				{
					instance.Initialize();					
					TryCreateBehavior();
				}
				
			}
		}

		public const string CONFIG_NAME = "se.hertzole.settingsmanager.settings";

		private void OnEnable()
		{
			if (instance == null)
			{
				instance = this;
				TryCreateBehavior();
			}

#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
		}

		public void Initialize()
		{
			if (isInitialized)
			{
				Debug.LogError($"{name} has already been initialized.");
				return;
			}

			instance = this;
			TryCreateBehavior();
		}

		/// <summary>
		/// Saves the settings to a file.
		/// </summary>
		public void SaveSettings()
		{
			behavior.SaveSettings();
		}

		/// <summary>
		/// Loads the settings from file.
		/// </summary>
		public void LoadSettings()
		{
			behavior.LoadSettings();
		}

		/// <summary>
		/// Only loads settings if they already haven't been loaded.
		/// </summary>
		public void LoadSettingsIfNeeded()
		{
			if (!HasLoadedSettings)
			{
				behavior.LoadSettings();
			}
		}

		public bool TryGetSetting(string identifier, out Setting setting)
		{
			if (cachedSettings.TryGetValue(identifier, out setting))
			{
				return true;
			}

			foreach (SettingsCategory category in categories)
			{
				foreach (BaseSetting baseSetting in category.Settings)
				{
					if(baseSetting is Setting newSetting && newSetting.Identifier == identifier)
					{
						setting = newSetting;
						cachedSettings.Add(identifier, newSetting);
						return true;
					}
				}
			}

			return false;
		}

		public void GetSerializeData(Dictionary<string, object> dataBuffer, IEnumerable<Setting> settings)
		{
			if (behavior != null)
			{
				SettingsManagerBehavior.GetSerializeData(dataBuffer, settings);
			}
		}

		public bool HasLoadedSetting(string identifier)
		{
			return behavior.HasLoadedSetting(identifier);
		}

		private static SettingsManager GetOrCreateSettings()
		{
			if (instance != null)
			{
				TryCreateBehavior();
				return instance;
			}

			SettingsManager settings;

#if UNITY_EDITOR
			EditorBuildSettings.TryGetConfigObject(CONFIG_NAME, out settings);
#else
			settings = FindObjectOfType<SettingsManager>();
#endif
			if (settings == null)
			{
				Debug.LogWarning("Could not find settings manager. Default settings will be used.");

				settings = CreateInstance<SettingsManager>();
				settings.name = "Settings Manager";
			}

			return settings;
		}

		private static void TryCreateBehavior()
		{
			if (instance == null || instance.behavior != null)
			{
				return;
			}

			CreateBehavior();
		}

		private static void CreateBehavior()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#endif

			if (instance == null)
			{
				return;
			}

			if (instance.behavior != null)
			{
				Destroy(instance.behavior.gameObject);
			}

			GameObject behaviorObject = new GameObject("Settings Manager Behavior");
			instance.behavior = behaviorObject.AddComponent<SettingsManagerBehavior>();
			DontDestroyOnLoad(behaviorObject);

			instance.behavior.Manager = instance;
			instance.behavior.Serializer = instance.Serializer;
			instance.behavior.AutoSaveSettings = instance.autoSaveSettings;
			instance.behavior.SavePath = GetSavePath(instance);

			if (instance.loadSettingsOnBoot)
			{
				instance.behavior.LoadSettings();
			}
			else
			{
				instance.behavior.SetDefaultValues();
			}
		}

		public static string GetSavePath(SettingsManager settings)
		{
			savePathBuilder.Clear();

			savePathBuilder.Append(GetSaveLocation(settings.saveLocation, settings.customPathProvider));

			if (!string.IsNullOrWhiteSpace(settings.savePath))
			{
				savePathBuilder.Append($"/{settings.savePath}");
			}

			savePathBuilder.Append($"/{settings.fileName}");

			return Path.GetFullPath(savePathBuilder.ToString());
		}

		public static string GetSaveLocation(SaveLocations location, ISettingPathProvider pathProvider)
		{
			switch (location)
			{
				case SaveLocations.PersistentDataPath:
					return Application.persistentDataPath;
				case SaveLocations.DataPath:
					return Application.dataPath;
				case SaveLocations.Documents:
					return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				case SaveLocations.Custom:
					return pathProvider != null ? pathProvider.GetSettingsPath() : Application.persistentDataPath;
				default:
#if DEBUG
					Debug.LogError($"{location} is an invalid save location. Returning persistent data path instead.");
#endif
					return Application.persistentDataPath;
			}
		}

#if UNITY_EDITOR
		private void OnDisable()
		{
			ResetState();
			EditorApplication.playModeStateChanged -= OnPlayModeChanged;
		}

		private void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
			{
				ResetState();
			}

			if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingEditMode)
			{
				TryCreateBehavior();
			}
		}

		private void ResetState()
		{
			isInitialized = false;
			cachedSettings.Clear();

			for (int i = 0; i < categories.Count; i++)
			{
				for (int j = 0; j < categories[i].Settings.Count; j++)
				{
					categories[i].Settings[j].ResetState();
				}
			}
		}
#endif
	}
}