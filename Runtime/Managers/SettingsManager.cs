using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
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
		private ISettingWriter fileWriter = new FileWriter();
		[SerializeReference] 
		private ISettingSerializer serializer = new JsonSettingSerializer();

		[SerializeField]
		private List<SettingsCategory> categories = new List<SettingsCategory>();

		private bool isInitialized = false;

		private readonly Dictionary<string, Setting> cachedSettings = new Dictionary<string, Setting>();

		private static SettingsManager instance;

		internal SettingsManagerBehavior behavior;

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
		public ISettingWriter FileWriter { get { return fileWriter; } set { fileWriter = value; } }
		public ISettingSerializer Serializer { get { return serializer; } set { serializer = value; } }
		public List<SettingsCategory> Categories { get { return categories; } set { categories = value; } }

		public static SettingsManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GetOrCreateSettings();
					instance.Initialize();
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
				InitializeCategories();
				TryCreateBehavior();
			}

#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
		}

		/// <summary>
		///     Initializes this settings manager and sets this as the active global instance.
		/// </summary>
		public void Initialize()
		{
			if (isInitialized)
			{
				return;
			}

			instance = this;
			TryCreateBehavior();
		}

		/// <summary>
		///     Initializes all the categories and their settings.
		/// </summary>
		private void InitializeCategories()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#endif
			
			for (int i = 0; i < categories.Count; i++)
			{
				categories[i].Initialize(this);
			}
		}

		/// <summary>
		///     Starts a coroutine on the scene behaviour.
		/// </summary>
		/// <param name="coroutine">The coroutine to start.</param>
		public void StartCoroutine(IEnumerator coroutine)
		{
			TryCreateBehavior();

			behavior.StartCoroutine(coroutine);
		}

		/// <summary>
		///	    Adds a new category to the list of categories.
		/// </summary>
		/// <param name="categoryName">The category name.</param>
		/// <param name="icon">The category icon.</param>
		/// <param name="initialize">If true, the category will be initialized right away after creation-</param>
		/// <returns>The new category.</returns>
		public SettingsCategory AddCategory(string categoryName, Sprite icon = null, bool initialize = true)
		{
			SettingsCategory category = new SettingsCategory
			{
				DisplayName = categoryName,
				Icon = icon
			};

			categories.Add(category);
			if (initialize)
			{
				category.Initialize(this);
			}

			return category;
		}

#if HERTZ_SETTINGS_LOCALIZATION
		/// <summary>
		///	    Adds a new category to the list of categories.
		/// </summary>
		/// <param name="localizedName">The category name.</param>
		/// <param name="icon">The category icon.</param>
		/// <param name="initialize">If true, the category will be initialized right away after creation-</param>
		/// <returns>The new category.</returns>
		public SettingsCategory AddCategory(LocalizedString localizedName, Sprite icon = null, bool initialize = true)
		{
			SettingsCategory category = AddCategory(string.Empty, icon, initialize);
			category.DisplayNameLocalized = localizedName;

			return category;
		}
#endif

		/// <summary>
		///     Saves the settings to a file.
		/// </summary>
		public void SaveSettings()
		{
			behavior.SaveSettings();
		}

		/// <summary>
		///     Loads the settings from file.
		/// </summary>
		public void LoadSettings()
		{
			behavior.LoadSettings();
		}

		/// <summary>
		///     Only loads settings if they already haven't been loaded.
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
					if (baseSetting is Setting newSetting && newSetting.Identifier == identifier)
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

			instance.InitializeCategories();
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
			return GetSaveLocation(settings.saveLocation, settings.customPathProvider, settings.savePath, settings.fileName);
		}

		public static string GetSaveLocation(SaveLocations location, ISettingPathProvider pathProvider, string path, string fileName)
		{
			switch (location)
			{
				case SaveLocations.PersistentDataPath:
					return Path.Combine(Application.persistentDataPath, path, fileName);
				case SaveLocations.DataPath:
					return Path.Combine(Application.dataPath, path, fileName);
				case SaveLocations.Documents:
					return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path, fileName);
				case SaveLocations.Custom:
					return pathProvider != null ? pathProvider.GetFullSettingsPath(path, fileName) : GetSaveLocation(SaveLocations.PersistentDataPath, null, path, fileName);
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
			if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode)
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