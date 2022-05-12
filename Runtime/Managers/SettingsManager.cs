using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hertzole.SettingsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Settings Manager", menuName = "Hertzole/Settings/Settings Manager", order = -10000)]
#endif
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
		public SaveLocations SaveLocation { get { return saveLocation; } set { saveLocation = value; } }
		public string SavePath { get { return savePath; } set { savePath = value; } }
		public string FileName { get { return fileName; } set { fileName = value; } }
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

		public void SaveSettings()
		{
			behavior.SaveSettings();
		}

		public void LoadSettings()
		{
			behavior.LoadSettings();
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

		public void GetSerializeData(Dictionary<string, object> dataBuffer)
		{
			if (behavior != null)
			{
				behavior.GetSerializeData(dataBuffer);
			}
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
		}

		public static string GetSavePath(SettingsManager settings)
		{
			savePathBuilder.Clear();

			switch (settings.saveLocation)
			{
				case SaveLocations.PersistentDataPath:
					savePathBuilder.Append(Application.persistentDataPath);
					break;
				case SaveLocations.DataPath:
					savePathBuilder.Append(Application.dataPath);
					break;
				case SaveLocations.Documents:
					savePathBuilder.Append(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
					break;
				case SaveLocations.Custom:
					savePathBuilder.Append(settings.customPathProvider != null ? settings.customPathProvider.GetSettingsPath() : Application.persistentDataPath);
					break;
				default:
					savePathBuilder.Append(Application.persistentDataPath);
					break;
			}

			if (!string.IsNullOrWhiteSpace(settings.savePath))
			{
				savePathBuilder.Append($"/{settings.savePath}");
			}

			savePathBuilder.Append($"/{settings.fileName}");

			return Path.GetFullPath(savePathBuilder.ToString());
		}

		internal class SettingsManagerBehavior : MonoBehaviour
		{
			private bool dirtySave;
			private bool startedListeningForSave;

			private readonly Dictionary<string, object> settingData = new Dictionary<string, object>();

			private float saveTime;
			private readonly HashSet<string> excludedSettings = new HashSet<string>();
			private SettingsManager manager;

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

			internal void SaveSettings()
			{
				dirtySave = false;

				string directory = Path.GetDirectoryName(SavePath);
				if (string.IsNullOrEmpty(directory))
				{
					return;
				}

				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				settingData.Clear();
				GetSerializeData(settingData);

				byte[] data = Serializer.Serialize(settingData);
				File.WriteAllBytes(SavePath, data);
			}

			internal void LoadSettings()
			{
				if (!File.Exists(SavePath))
				{
					return;
				}

				settingData.Clear();
				excludedSettings.Clear();

				byte[] data = File.ReadAllBytes(SavePath);
				Serializer.Deserialize(data, settingData);
				Dictionary<string, object>.Enumerator enumerator = settingData.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, object> current = enumerator.Current;
					if (Manager.TryGetSetting(current.Key, out Setting setting))
					{
						setting.SetSerializedValue(current.Value, Serializer);
						excludedSettings.Add(current.Key);
					}
				}

				enumerator.Dispose();
				SetDefaultValues(excludedSettings);
			}

			internal void GetSerializeData(Dictionary<string, object> dataBuffer)
			{
				for (int i = 0; i < Manager.categories.Count; i++)
				{
					for (int j = 0; j < Manager.categories[i].Settings.Count; j++)
					{
						if (!Manager.categories[i].Settings[j].CanSave)
						{
							continue;
						}

						dataBuffer[Manager.categories[i].Settings[j].Identifier] = Manager.categories[i].Settings[j].GetSerializeValue();
					}
				}
			}

			private void SetDefaultValues(ICollection<string> excluding)
			{
				for (int i = 0; i < manager.categories.Count; i++)
				{
					for (int j = 0; j < manager.categories[i].Settings.Count; j++)
					{
						if (!excluding.Contains(manager.categories[i].Settings[j].Identifier))
						{
							manager.categories[i].Settings[j].SetSerializedValue(manager.categories[i].Settings[j].GetDefaultValue(), Serializer);
						}
					}
				}
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
		}
#endif
	}
}