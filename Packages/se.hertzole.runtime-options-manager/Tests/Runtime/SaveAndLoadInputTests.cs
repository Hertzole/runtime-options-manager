#if HERTZ_SETTINGS_INPUTSYSTEM
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public partial class SaveAndLoadTests
	{
		private static readonly int[] bindingIndex = { 1, 2 };

		[Test]
		public void EmptyBindingLoadTest([ValueSource(nameof(bindingIndex))] int targetBinding)
		{
			InputSetting setting = AddSetting<InputSetting>();
			setting.TargetAction = ScriptableObject.CreateInstance<InputActionReference>();

			InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();

			asset.AddControlScheme(new InputControlScheme("Keyboard", new[]
			{
				new InputControlScheme.DeviceRequirement
					{ controlPath = "<Keyboard>", isOptional = false, isOR = false, isAND = true }
			}));

			InputActionMap actionMap = asset.AddActionMap("Test Map");
			actionMap.AddBinding(new InputBinding("1DAxis", "Test", "Keyboard", name: "Keyboard") { isComposite = true });
			actionMap.AddBinding(new InputBinding("<Keyboard>/a", "Test", "Keyboard", name: "negative") { isPartOfComposite = true });
			actionMap.AddBinding(new InputBinding("<Keyboard>/d", "Test", "Keyboard", name: "positive") { isPartOfComposite = true });
			InputAction action = actionMap.AddAction("Test");

			setting.TargetAction.Set(action);
			setting.Bindings = new[]
			{
				new InputSetting.BindingInfo(action.bindings[1].id.ToString(), "Keyboard"),
				new InputSetting.BindingInfo(action.bindings[2].id.ToString(), "Keyboard")
			};

			Assert.AreEqual(setting.Bindings[0].BindingId, action.bindings[1].id.ToString());
			Assert.AreEqual(setting.Bindings[1].BindingId, action.bindings[2].id.ToString());

			Assert.AreEqual(action.bindings[1].effectivePath, "<Keyboard>/a");
			Assert.AreEqual(action.bindings[2].effectivePath, "<Keyboard>/d");

			action.ApplyBindingOverride(targetBinding, "");

			Assert.AreEqual(action.bindings[targetBinding].effectivePath, "");

			SaveAndLoad(() =>
			{
				object serializeData = setting.GetSerializeValue();

				if (!(serializeData is InputActionData actionData))
				{
					NUnit.Framework.Assert.Fail("Serialized data is not an InputActionData");
					return;
				}

				Assert.AreEqual(actionData.bindings[targetBinding - 1].id, action.bindings[targetBinding].id.ToString());
				Assert.IsTrue(actionData.bindings[targetBinding - 1].isOverwritten);
				Assert.AreEqual(actionData.bindings[targetBinding - 1].path, "");
			}, () =>
			{
				action.ApplyBindingOverride(targetBinding, "<Keyboard>/p");

				Assert.AreEqual(action.bindings[targetBinding].effectivePath, "<Keyboard>/p");
			}, () =>
			{
				Assert.AreNotEqual(action.bindings[targetBinding].effectivePath, "<Keyboard>/p");
				Assert.AreEqual(action.bindings[targetBinding].effectivePath, "");
				Debug.Log(asset.ToJson());
			});
		}
	}
}
#endif