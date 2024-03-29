﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.OptionsManager.Editor
{
	[CustomEditor(typeof(SettingsManager))]
	public class SettingsManagerEditor : UnityEditor.Editor
	{
		private Label finalPathLabel;
		private readonly List<Type> availablePathProviders = new List<Type>();
		private readonly List<Type> availableSerializers = new List<Type>();
		private readonly List<Type> availableFileWriters = new List<Type>();

		private FoldoutPopupField<Type> pathProviderField;
		private FoldoutPopupField<Type> serializerField;
		private FoldoutPopupField<Type> fileWriterField;

		private SerializedProperty autoSaveSettings;
		private SerializedProperty categories;
		private SerializedProperty customPathProvider;
		private SerializedProperty fileWriter;
		private SerializedProperty fileName;
		private SerializedProperty loadSettingsOnBoot;
		private SerializedProperty saveLocation;
		private SerializedProperty savePath;
		private SerializedProperty serializer;

		private SettingsManager settingsManager;
		private int selectedPathProvider;

		private Type selectedSerializer;
		private Type selectedFileWriter;
		
		private VisualElement serializerSettings;
		private VisualElement pathProviderSettings;
		private VisualElement fileWriterSettings;

		private void OnEnable()
		{
			autoSaveSettings = serializedObject.FindProperty(nameof(autoSaveSettings));
			loadSettingsOnBoot = serializedObject.FindProperty(nameof(loadSettingsOnBoot));
			saveLocation = serializedObject.FindProperty(nameof(saveLocation));
			savePath = serializedObject.FindProperty(nameof(savePath));
			fileName = serializedObject.FindProperty(nameof(fileName));
			customPathProvider = serializedObject.FindProperty(nameof(customPathProvider));
			fileWriter = serializedObject.FindProperty(nameof(fileWriter));
			serializer = serializedObject.FindProperty(nameof(serializer));
			categories = serializedObject.FindProperty(nameof(categories));

			settingsManager = (SettingsManager) target;

			availableSerializers.Clear();
			availableFileWriters.Clear();
			
			availableSerializers.AddRange(GetAvailableTypes<ISettingSerializer>(serializer, out selectedSerializer));
			availableFileWriters.AddRange(GetAvailableTypes<ISettingWriter>(fileWriter, out selectedFileWriter));
			
			GetPathProviders();

			if (serializer.managedReferenceValue == null)
			{
				serializer.managedReferenceValue = new JsonSettingSerializer();
				serializedObject.ApplyModifiedProperties();
			}

			if (fileWriter.managedReferenceValue == null)
			{
				fileWriter.managedReferenceValue = new FileWriter();
				serializedObject.ApplyModifiedProperties();
			}
		}

		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();

			serializerField = new FoldoutPopupField<Type>("Serializer", availableSerializers, selectedSerializer, FormatTypeName, FormatTypeName)
			{
				IsExpanded = serializer.isExpanded,
			};
			
			serializerField.RegisterValueChangedCallback(ctx =>
			{
				serializer.managedReferenceValue = Activator.CreateInstance(ctx.newValue);
				serializedObject.ApplyModifiedProperties();
				selectedSerializer = ctx.newValue;

				UpdateSerializerSettings();
			});
			
			serializerField.RegisterFoldoutCallback(ctx =>
			{
				serializer.isExpanded = ctx.newValue;
			});

			pathProviderField = new FoldoutPopupField<Type>("Path Provider", availablePathProviders, selectedPathProvider, FormatTypeName, FormatTypeName)
			{
				IsExpanded = customPathProvider.isExpanded
			};
			
			pathProviderField.RegisterValueChangedCallback(ctx =>
			{
				customPathProvider.managedReferenceValue = ctx.newValue == null ? null : Activator.CreateInstance(ctx.newValue);
				serializedObject.ApplyModifiedProperties();
				selectedPathProvider = availablePathProviders.IndexOf(ctx.newValue);
				
				UpdatePathProviderSettings();
				UpdateFinalPath();
			});
			
			pathProviderField.RegisterFoldoutCallback(ctx =>
			{
				customPathProvider.isExpanded = ctx.newValue;
			});

			pathProviderField.style.display = saveLocation.enumValueIndex == 3 ? DisplayStyle.Flex : DisplayStyle.None;

			fileWriterField = new FoldoutPopupField<Type>("File Writer", availableFileWriters, selectedFileWriter, FormatTypeName, FormatTypeName)
			{
				IsExpanded = fileWriter.isExpanded
			};

			fileWriterField.RegisterValueChangedCallback(ctx =>
			{
				fileWriter.managedReferenceValue = ctx.newValue == null ? null : Activator.CreateInstance(ctx.newValue);
				serializedObject.ApplyModifiedProperties();
				selectedFileWriter = ctx.newValue;
				
				UpdateFileWriterSettings();
			});
			
			fileWriterField.RegisterFoldoutCallback(ctx =>
			{
				fileWriter.isExpanded = ctx.newValue;
			});

			Toggle autoSaveSettingsField = new Toggle(autoSaveSettings.displayName);
			autoSaveSettingsField.BindProperty(autoSaveSettings);

			Toggle loadSettingsOnBootField = new Toggle(loadSettingsOnBoot.displayName);
			loadSettingsOnBootField.BindProperty(loadSettingsOnBoot);

			EnumField saveLocationField = new EnumField(saveLocation.displayName, (SettingsManager.SaveLocations) saveLocation.enumValueIndex);
			saveLocationField.BindProperty(saveLocation);
			saveLocationField.RegisterValueChangedCallback(ctx =>
			{
				UpdateFinalPath();

				if (ctx == null || ctx.newValue == null)
				{
					return;
				}
				
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

			MakePropertyField(autoSaveSettingsField);
			MakePropertyField(loadSettingsOnBootField);
			MakePropertyField(saveLocationField);
			MakePropertyField(pathProviderField);
			MakePropertyField(fileWriterField);
			MakePropertyField(savePathField);
			MakePropertyField(fileNameField);
			MakePropertyField(serializerField);
			
			UpdateFinalPath();
			root.Add(CreateHeader("Saving And Loading"));
			root.Add(autoSaveSettingsField);
			root.Add(loadSettingsOnBootField);
			root.Add(saveLocationField);
			root.Add(pathProviderField);
			root.Add(fileWriterField);
			root.Add(savePathField);
			root.Add(fileNameField);
			root.Add(finalPathLabel);
			root.Add(serializerField);

			root.Add(CreateSpace());

			// root.Add(categoriesList);
			root.Add(new IMGUIContainer(() =>
			{
				serializedObject.Update();
				EditorGUILayout.PropertyField(categories, true);
				serializedObject.ApplyModifiedProperties();
			}));

			root.Add(CreateSpace(16));

			UpdateSerializerSettings();
			UpdatePathProviderSettings();
			UpdateFileWriterSettings();

			return root;
		}

		private static void MakePropertyField(VisualElement element)
		{
			element.AddToClassList("unity-base-field__aligned");
		}

		private void UpdateSerializerSettings()
		{
			if (serializerSettings != null)
			{
				serializerSettings.RemoveFromHierarchy();
				serializerSettings = null;
			}

			serializerSettings = CreateSerializerEditor(serializer);

			if (serializerSettings != null)
			{
				serializerField.AddToFoldout(serializerSettings);
			}

			serializerField.ShowFoldout = serializerSettings != null;
		}

		private void UpdatePathProviderSettings()
		{
			if (pathProviderSettings != null)
			{
				pathProviderSettings.RemoveFromHierarchy();
				pathProviderSettings = null;
			}

			pathProviderSettings = CreateSerializerEditor(customPathProvider, evt =>
			{
				UpdateFinalPath();
			});
			
			if (pathProviderSettings != null)
			{
				pathProviderField.AddToFoldout(pathProviderSettings);
			}
			
			pathProviderField.ShowFoldout = pathProviderSettings != null;
		}

		private void UpdateFileWriterSettings()
		{
			if (fileWriterSettings != null)
			{
				fileWriterSettings.RemoveFromHierarchy();
				fileWriterSettings = null;
			}

			fileWriterSettings = CreateSerializerEditor(fileWriter);
			
			if (fileWriterSettings != null)
			{
				fileWriterField.AddToFoldout(fileWriterSettings);
			}
			
			fileWriterField.ShowFoldout = fileWriterSettings != null;
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

		private static VisualElement CreateSerializerEditor(SerializedProperty property, EventCallback<SerializedPropertyChangeEvent> onAnyValueChanged = null)
		{
			VisualElement root = new VisualElement
			{
				name = property.type + "-settings"
			};


			SerializedProperty propertyFields = property.Copy();
			SerializedProperty lastProperty = propertyFields.GetEndProperty();

			int count = 0;

			while (propertyFields.NextVisible(true))
			{
				if (propertyFields.name == lastProperty.name)
				{
					break;
				}

				PropertyField propField = new PropertyField(propertyFields, propertyFields.displayName);
				propField.Bind(property.serializedObject);
				root.Add(propField);
				if (onAnyValueChanged != null)
				{
					propField.RegisterValueChangeCallback(onAnyValueChanged);
				}
				count++;
			}

			return count == 0 ? null : root;
		}

		private static string FormatTypeName(Type arg)
		{
			return arg == null ? "None" : arg.Name;
		}

		private static Type[] GetAvailableTypes<T>(SerializedProperty property, out Type selectedType)
		{
			TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<T>();
			Type[] result = new Type[types.Count];

			types.CopyTo(result, 0);

			selectedType = null;
			
			if (property != null && property.managedReferenceValue != null)
			{
				for (int i = 0; i < types.Count; i++)
				{
					if (types[i] == property.managedReferenceValue.GetType())
					{
						selectedType = types[i];
						break;
					}
				}
			}

			return result;
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
						selectedPathProvider = i + 1;
						break;
					}
				}
			}
		}
	}
}