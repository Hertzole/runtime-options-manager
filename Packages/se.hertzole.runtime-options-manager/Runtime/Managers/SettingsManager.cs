using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hertzole.RuntimeOptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Settings Manager", menuName = "Hertzole/Settings/Settings Manager", order = -10000)]
#endif
	[DefaultExecutionOrder(-1000000)]
	public class SettingsManager : ScriptableObject
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

			for (int i = 0; i < categories.Count; i++)
			{
				for (int j = 0; j < categories[i].Settings.Count; j++)
				{
					if (categories[i].Settings[j].Identifier == identifier)
					{
						cachedSettings.Add(categories[i].Settings[j].Identifier, categories[i].Settings[j]);
						setting = categories[i].Settings[j];
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
				for (int i = 0; i < Manager.categories.Count; i++)
				{
					for (int j = 0; j < Manager.categories[i].Settings.Count; j++)
					{
						if (listen)
						{
							Manager.categories[i].Settings[j].OnSettingChanged += OnSettingChanged;
						}
						else
						{
							Manager.categories[i].Settings[j].OnSettingChanged -= OnSettingChanged;
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
				Debug.Log("Load settings routine");
				
// #if HERTZ_SETTINGS_LOCALIZATION
// 				AsyncOperationHandle<LocalizationSettings> localizationOperation = LocalizationSettings.Instance.GetInitializationOperation();
// 				while (!localizationOperation.IsDone)
// 				{
// 					Debug.Log("Not done...");
// 					yield return null;
// 				}
// #endif

				Debug.Log("Get save paths");
				
				GetSavePaths();

				foreach (string settingPath in settingPaths.Keys)
				{
					Debug.Log("Loading settings from " + settingPath);
					
					if(!File.Exists(settingPath))
					{
						continue;
					}

					Debug.Log("Read bytes");
					byte[] data = File.ReadAllBytes(settingPath);
					Debug.Log("Deserialize " + data + " | " + settingPath);
					Serializer.Deserialize(data, settingData);
					Debug.Log("Get enumerator");
					Dictionary<string, object>.Enumerator enumerator = settingData.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Debug.Log("Move");
						
						KeyValuePair<string, object> current = enumerator.Current;

						Debug.Log($"Try get setting {current.Key}");
						if (Manager.TryGetSetting(current.Key, out Setting setting))
						{
							Debug.Log("Found setting");
							loadedSettings.Add(current.Key);
							Debug.Log($"Loaded setting {current.Key}. Setting serialized value to {current.Value}");
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
					foreach (Setting setting in category.Settings)
					{
						if (!setting.CanSave)
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
				for (int i = 0; i < manager.categories.Count; i++)
				{
					for (int j = 0; j < manager.categories[i].Settings.Count; j++)
					{
						if (excluding == null || !excluding.Contains(manager.categories[i].Settings[j].Identifier))
						{
							manager.categories[i].Settings[j].SetSerializedValue(manager.categories[i].Settings[j].GetDefaultValue(), Serializer);
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