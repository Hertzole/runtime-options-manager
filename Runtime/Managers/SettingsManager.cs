using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Hertzole.Settings
{
	[DefaultExecutionOrder(-10000)]
	public class SettingsManager : MonoBehaviour
	{
		[SerializeField]
		private SettingsObject settings = default;
		[SerializeField]
		private bool singleton = true;

		[Header("Saving")]
		[SerializeField]
		private bool autoSaveSettings = true;
		[SerializeField]
		private bool prettyPrint = false;

		[Header("Loading")]
		[SerializeField]
		private bool loadOnStart = true;

		private bool dirtySave;

		private readonly Dictionary<string, object> settingData = new Dictionary<string, object>();

		private float saveTime;
		
		private readonly List<string> excludedSettings = new List<string>();

		private static SettingsManager instance;

		private Formatting JsonFormat { get { return prettyPrint ? Formatting.Indented : Formatting.None; } }

		public ISettingSerializer Serializer { get; set; } = new SettingSerializer();

		private void Awake()
		{
			if (singleton)
			{
				if (instance != null && instance != this)
				{
					Destroy(gameObject);
					return;
				}

				instance = this;
				DontDestroyOnLoad(gameObject);
			}
		}

		private void Start()
		{
			if (loadOnStart)
			{
				LoadSettings();
			}
			else
			{
				SetDefaultValues();
			}
		}

		private void Update()
		{
			if (autoSaveSettings && dirtySave && Time.unscaledTime >= saveTime)
			{
				SaveSettings();
			}
		}

		private void OnEnable()
		{
			for (int i = 0; i < settings.Categories.Length; i++)
			{
				for (int j = 0; j < settings.Categories[i].Settings.Length; j++)
				{
					settings.Categories[i].Settings[j].OnSettingChanged += OnAnySettingChanged;
				}
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < settings.Categories.Length; i++)
			{
				for (int j = 0; j < settings.Categories[i].Settings.Length; j++)
				{
					settings.Categories[i].Settings[j].OnSettingChanged -= OnAnySettingChanged;
				}
			}
		}

		private void OnDestroy()
		{
			if (singleton && instance == this)
			{
				instance = null;
			}
		}

		private void OnAnySettingChanged()
		{
			dirtySave = true;
			saveTime = Time.unscaledTime + 1f;
		}

		public void SaveSettings()
		{
			if (!dirtySave)
			{
				return;
			}

			dirtySave = false;
			Serializer.FillData(settings, settingData);
			string json = Serializer.SerializeToJson(settingData, JsonFormat);

			File.WriteAllText(Application.persistentDataPath + "/settings.json", json);
		}

		public void LoadSettings()
		{
			if (!File.Exists(Application.persistentDataPath + "/settings.json"))
			{
				SetDefaultValues();
				return;
			}

			excludedSettings.Clear();

			Serializer.DeserializeFromJson(File.ReadAllText(Application.persistentDataPath + "/settings.json"), settingData);
			foreach (KeyValuePair<string, object> setting in settingData)
			{
				if (settings.TryGetSetting(setting.Key, out Setting settingObject))
				{
					settingObject.SetSerializedValue(setting.Value);
					excludedSettings.Add(setting.Key);
				}
			}

			SetDefaultValues(excludedSettings);
		}

		public void SetDefaultValues(IList<string> excluding = null)
		{
			for (int i = 0; i < settings.Categories.Length; i++)
			{
				for (int j = 0; j < settings.Categories[i].Settings.Length; j++)
				{
					if (excluding == null || !excluding.Contains(settings.Categories[i].Settings[j].Identifier))
					{
						settings.Categories[i].Settings[j].SetSerializedValue(settings.Categories[i].Settings[j].GetDefaultValue());
					}
				}
			}
		}

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			instance = null;
		}
#endif
	}
}