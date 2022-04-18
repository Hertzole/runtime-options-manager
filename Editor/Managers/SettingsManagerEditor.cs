using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.SettingsManager.Editor
{
	[CustomEditor(typeof(SettingsManager))]
	public class SettingsManagerEditor : UnityEditor.Editor
	{
		private Foldout serializerSettingsFoldout;
		private Label finalPathLabel;
		private readonly List<Type> availablePathProviders = new List<Type>();

		private readonly List<Type> availableSerializers = new List<Type>();
		private PopupField<Type> pathProviderField;

		private PopupField<Type> serializerField;
		private SerializedProperty autoSaveSettings;
		private SerializedProperty categories;
		private SerializedProperty customPathProvider;
		private SerializedProperty fileName;
		private SerializedProperty loadSettingsOnBoot;
		private SerializedProperty saveLocation;
		private SerializedProperty savePath;
		private SerializedProperty serializer;

		private SettingsManager settingsManager;
		private Type selectedPathProvider;

		private Type selectedSerializer;
		private VisualElement serializerSettings;

		private void OnEnable()
		{
			autoSaveSettings = serializedObject.FindProperty(nameof(autoSaveSettings));
			loadSettingsOnBoot = serializedObject.FindProperty(nameof(loadSettingsOnBoot));
			saveLocation = serializedObject.FindProperty(nameof(saveLocation));
			savePath = serializedObject.FindProperty(nameof(savePath));
			fileName = serializedObject.FindProperty(nameof(fileName));
			customPathProvider = serializedObject.FindProperty(nameof(customPathProvider));
			serializer = serializedObject.FindProperty(nameof(serializer));
			categories = serializedObject.FindProperty(nameof(categories));

			settingsManager = (SettingsManager) target;

			GetSerializerTypes();
			GetPathProviders();

			if (serializer.managedReferenceValue == null)
			{
				serializer.managedReferenceValue = Activator.CreateInstance(availableSerializers[0]);
				serializedObject.ApplyModifiedProperties();
			}
		}

		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();

			serializerField = new PopupField<Type>("Serializer", availableSerializers, selectedSerializer, FormatTypeName, FormatTypeName);
			serializerField.RegisterValueChangedCallback(ctx =>
			{
				serializer.managedReferenceValue = Activator.CreateInstance(ctx.newValue);
				serializedObject.ApplyModifiedProperties();
				selectedSerializer = ctx.newValue;

				UpdateSerializerSettings();
			});

			pathProviderField = new PopupField<Type>("Path Provider", availablePathProviders, 0, FormatTypeName, FormatTypeName);
			pathProviderField.RegisterValueChangedCallback(ctx =>
			{
				customPathProvider.managedReferenceValue = ctx.newValue == null ? null : Activator.CreateInstance(ctx.newValue);
				serializedObject.ApplyModifiedProperties();
				selectedPathProvider = ctx.newValue;
			});

			pathProviderField.style.display = saveLocation.enumValueIndex == 3 ? DisplayStyle.Flex : DisplayStyle.None;

			serializerSettingsFoldout = new Foldout
			{
				text = "Settings",
				value = serializer.isExpanded,
				style =
				{
					marginLeft = 16
				}
			};

			serializerSettingsFoldout.RegisterValueChangedCallback(ctx => { serializer.isExpanded = ctx.newValue; });

			Toggle autoSaveSettingsField = new Toggle(autoSaveSettings.displayName);
			autoSaveSettingsField.BindProperty(autoSaveSettings);

			Toggle loadSettingsOnBootField = new Toggle(loadSettingsOnBoot.displayName);
			loadSettingsOnBootField.BindProperty(loadSettingsOnBoot);

			EnumField saveLocationField = new EnumField(saveLocation.displayName, (SettingsManager.SaveLocations) saveLocation.enumValueIndex);
			saveLocationField.BindProperty(saveLocation);
			saveLocationField.RegisterValueChangedCallback(ctx =>
			{
				UpdateFinalPath();

				SettingsManager.SaveLocations newValue = (SettingsManager.SaveLocations) ctx.newValue;

				pathProviderField.style.display = newValue == SettingsManager.SaveLocations.Custom ? DisplayStyle.Flex : DisplayStyle.None;
			});

			TextField savePathField = new TextField(savePath.displayName);
			savePathField.BindProperty(savePath);
			savePathField.RegisterValueChangedCallback(_ => { UpdateFinalPath(); });

			TextField fileNameField = new TextField(fileName.displayName);
			fileNameField.BindProperty(fileName);
			fileNameField.RegisterValueChangedCallback(_ => { UpdateFinalPath(); });

			finalPathLabel = new Label
			{
				style =
				{
					flexWrap = Wrap.Wrap,
					marginLeft = 3,
					marginTop = 2,
					marginBottom = 4
				}
			};

			// The list view is still a bit buggy, so we need to use an IMGUI container instead.
			// ListView categoriesList = new ListView()
			// {
			// 	showBorder = true,
			// 	fixedItemHeight = 100,
			// 	showFoldoutHeader = true,
			// 	headerTitle = categories.displayName,
			// 	showAddRemoveFooter = true,
			// 	reorderable = true,
			// 	reorderMode = ListViewReorderMode.Animated,
			// 	selectionType = SelectionType.Single,
			// 	showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
			// 	virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
			// };
			// categoriesList.BindProperty(categories);

			UpdateFinalPath();
			root.Add(CreateHeader("Saving And Loading"));
			root.Add(autoSaveSettingsField);
			root.Add(loadSettingsOnBootField);
			root.Add(saveLocationField);
			root.Add(pathProviderField);
			root.Add(savePathField);
			root.Add(fileNameField);
			root.Add(finalPathLabel);
			root.Add(serializerField);
			root.Add(serializerSettingsFoldout);

			root.Add(CreateSpace());

			// root.Add(categoriesList);
			root.Add(new IMGUIContainer(() => { EditorGUILayout.PropertyField(categories, true); }));

			UpdateSerializerSettings();

			return root;
		}

		private void UpdateSerializerSettings()
		{
			if (serializerSettings != null)
			{
				serializerSettings.RemoveFromHierarchy();
				serializerSettings = null;
			}

			serializerSettings = CreateSerializerEditor(serializer);
			serializerSettingsFoldout.Add(serializerSettings);
		}

		private void UpdateFinalPath()
		{
			finalPathLabel.text = SettingsManager.GetSavePath(settingsManager).Replace("\\", "/");
		}

		private static Label CreateHeader(string text)
		{
			return new Label(text)
			{
				style =
				{
					unityFontStyleAndWeight = FontStyle.Bold,
					marginLeft = 3,
					marginTop = 6
				}
			};
		}

		private static VisualElement CreateSpace(float space = 8f)
		{
			return new VisualElement
			{
				name = "space",
				style =
				{
					height = space
				}
			};
		}

		private static VisualElement CreateSerializerEditor(SerializedProperty property)
		{
			VisualElement root = new VisualElement();

			SerializedProperty propertyFields = property.Copy();
			SerializedProperty lastProperty = propertyFields.GetEndProperty();

			while (propertyFields.NextVisible(true))
			{
				if (propertyFields.name == lastProperty.name)
				{
					break;
				}

				PropertyField propField = new PropertyField(propertyFields, propertyFields.displayName);
				propField.Bind(property.serializedObject);
				root.Add(propField);
			}

			return root;
		}

		private static string FormatTypeName(Type arg)
		{
			return arg == null ? "None" : arg.Name;
		}

		private void GetSerializerTypes()
		{
			availableSerializers.Clear();

			TypeCache.TypeCollection serializers = TypeCache.GetTypesDerivedFrom<ISettingSerializer>();
			availableSerializers.AddRange(serializers);

			if (serializer.managedReferenceValue != null)
			{
				for (int i = 0; i < serializers.Count; i++)
				{
					if (serializers[i] == serializer.managedReferenceValue.GetType())
					{
						selectedSerializer = serializers[i];
						break;
					}
				}
			}

			selectedSerializer ??= serializers[0];
		}

		private void GetPathProviders()
		{
			availablePathProviders.Clear();

			availablePathProviders.Add(null);

			TypeCache.TypeCollection pathProviders = TypeCache.GetTypesDerivedFrom<ISettingPathProvider>();
			availablePathProviders.AddRange(pathProviders);

			if (customPathProvider.managedReferenceValue != null)
			{
				for (int i = 0; i < pathProviders.Count; i++)
				{
					if (pathProviders[i] == customPathProvider.managedReferenceValue.GetType())
					{
						selectedPathProvider = pathProviders[i];
						break;
					}
				}
			}
		}
	}
}