using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.OptionsManager.Editor
{
	public class FoldoutPopupField<T> : VisualElement, INotifyValueChanged<T>
	{
		private bool showFoldout;

		private float labelWidthRatio;
		private float labelExtraPadding;
		private float labelBaseMinWidth;

		private int cachedListAndFoldoutDepth;

		private VisualElement cachedInspectorElement;
		
		private readonly VisualElement fields;
		private readonly PopupField<T> popupElement;
		private readonly Foldout foldoutElement;
		private readonly VisualElement container;
		private readonly Label foldoutLabelElement;

		public bool ShowFoldout
		{
			get { return showFoldout; }
			set
			{
				if (showFoldout != value)
				{
					showFoldout = value;
					UpdateFoldout();
				}
			}
		}

		public bool IsExpanded { get { return foldoutElement.value; } set { foldoutElement.value = value; } }

		public T value { get { return popupElement.value; } set { popupElement.value = value; } }

		private FoldoutPopupField(string label)
		{
			fields = new VisualElement
			{
				style =
				{
					marginLeft = 0,
					marginRight = 0
				}
			};

			fields.AddToClassList("unity-base-field");

			foldoutElement = new Foldout
			{
				text = label,
			};

			foldoutElement.AddToClassList("unity-base-field__label");

			fields.Add(foldoutElement);

			Add(fields);

			container = foldoutElement.contentContainer;
			container.RemoveFromHierarchy();
			Add(container);
			
			foldoutLabelElement = foldoutElement.Q<Toggle>().Q<Label>(null, "unity-foldout__text");
			foldoutLabelElement.AddToClassList("unity-base-field__label");
			

			RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
		}

		public FoldoutPopupField(string label,
			List<T> choices,
			int defaultIndex,
			Func<T, string> formatSelectedValueCallback = null,
			Func<T, string> formatListItemCallback = null) : this(label)
		{
			popupElement = new PopupField<T>(label, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
			{
				style =
				{
					flexGrow = 1
				}
			};

			fields.Add(popupElement);
			UpdateFoldout();
		}

		public FoldoutPopupField(string label,
			List<T> choices,
			T defaultValue,
			Func<T, string> formatSelectedValueCallback = null,
			Func<T, string> formatListItemCallback = null) : this(label)
		{
			popupElement = new PopupField<T>(label, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback)
			{
				style =
				{
					flexGrow = 1
				}
			};

			fields.Add(popupElement);
			UpdateFoldout();
		}

		private void UpdateFoldout()
		{
			foldoutElement.style.display = showFoldout ? DisplayStyle.Flex : DisplayStyle.None;
			popupElement.labelElement.style.display = showFoldout ? DisplayStyle.None : DisplayStyle.Flex;
		}
		
		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			VisualElement currentElement = parent;
			while (currentElement != null)
			{
				if (currentElement.ClassListContains("unity-inspector-element"))
				{
					labelWidthRatio = 0.45f;
					labelExtraPadding = 2.0f;
					labelBaseMinWidth = 120.0f;

					cachedListAndFoldoutDepth = GetListAndFoldoutDepth(this);
					cachedInspectorElement = currentElement;

					RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
					break;
				}

				currentElement = currentElement.parent;
			}
		}

		private void OnGeometryChanged(GeometryChangedEvent evt)
		{
			AlignElement(foldoutLabelElement);
			AlignElement(popupElement.labelElement);
		}

		private void AlignElement(VisualElement element, float offset = 0)
		{
			if (!ClassListContains("unity-base-field__aligned"))
			{
				return;
			}

			var listAndFoldoutMargin = 15 * cachedListAndFoldoutDepth;
			
			// Get inspector element padding.
			var totalPadding = cachedInspectorElement.resolvedStyle.paddingLeft + cachedInspectorElement.resolvedStyle.paddingRight + cachedInspectorElement.resolvedStyle.marginLeft + cachedInspectorElement.resolvedStyle.marginRight;
			
			// Then get label padding.
			totalPadding += element.resolvedStyle.paddingLeft + element.resolvedStyle.paddingRight + element.resolvedStyle.marginLeft + element.resolvedStyle.marginRight;
			
			// Then get base field padding.
			totalPadding += resolvedStyle.paddingLeft + resolvedStyle.paddingRight + resolvedStyle.marginLeft + resolvedStyle.marginRight;

			totalPadding += labelExtraPadding;
			totalPadding += listAndFoldoutMargin;

			element.style.minWidth = Mathf.Max(labelBaseMinWidth - 15, 0);
			
			var newWidth = cachedInspectorElement.resolvedStyle.width * labelWidthRatio - totalPadding;
			newWidth += offset;
			if (Mathf.Abs(element.resolvedStyle.width - newWidth) > 1.0E-30F)
			{
				element.style.width = Mathf.Max(0, newWidth);
			}
		}

		private static int GetListAndFoldoutDepth(VisualElement element)
		{
			var depth = 0;
			if(element.hierarchy.parent != null)
			{
				var currentParent = element.hierarchy.parent;
				while (currentParent != null)
				{
					var currentParentType = currentParent.GetType();
					if (currentParent is Foldout || currentParent is ListView)
					{
						depth++;
					}
					
					currentParent = currentParent.hierarchy.parent;
				}
			}
			
			return depth;
		}

		public void RegisterFoldoutCallback(EventCallback<ChangeEvent<bool>> callback)
		{
			foldoutElement.RegisterValueChangedCallback(ctx =>
			{
				if (ctx.target == foldoutElement)
				{
					callback.Invoke(ctx);
				}
			});
		}

		public void AddToFoldout(VisualElement element)
		{
			container.Add(element);
		}

		public void SetValueWithoutNotify(T newValue)
		{
			popupElement.SetValueWithoutNotify(newValue);
		}
	}
}