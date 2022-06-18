#if HERTZ_SETTINGS_INPUTSYSTEM
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Hertzole.RuntimeOptionsManager.Tests
{
	public partial class SerializerTest<T>
	{
		private PlayerInput player;
		private InputActionReference actionReference;
		private readonly InputTestFixture input = new InputTestFixture();

		private void InputSetup()
		{
			input.Setup();

			InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
			InputActionMap map = asset.AddActionMap("TestMap");
			InputAction action = map.AddAction("TestAction");

			action.AddBinding("<Keyboard>/space");
			action.AddBinding("Gamepad/buttonSouth");

			actionReference = ScriptableObject.CreateInstance<InputActionReference>();
			actionReference.Set(action);

			player = new GameObject("Player").AddComponent<PlayerInput>();
			player.actions = asset;
			objects.Add(player.gameObject);
		}

		private void InputTearDown()
		{
			input.TearDown();

			Object.DestroyImmediate(actionReference);
		}

		[UnityTest]
		public IEnumerator Serialize_InputSetting()
		{
			InputSetting setting = AddSetting<InputSetting>();
			setting.Identifier = "setting";
			setting.TargetAction = actionReference;
			InputBinding[] bindings = setting.TargetAction.action.bindings.ToArray();
			setting.Bindings = new[]
			{
				new InputSetting.BindingInfo(bindings[0].id.ToString(), bindings[0].groups),
				new InputSetting.BindingInfo(bindings[1].id.ToString(), bindings[1].groups)
			};

			Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

			bool completed = false;

			InputActionRebindingExtensions.RebindingOperation operation = setting.StartRebind(player, out bool isComposite, out int bindingIndex);
			operation.OnComplete(o =>
			{
				completed = true;
			});

			operation.Start();

			yield return null;

			input.Press(keyboard.jKey);

			while (!completed)
			{
				yield return null;
			}

			setting.FinishRebind();
			operation.Dispose();

			yield return null;

			const string j = "<Keyboard>/j";
			Assert.AreEqual(j, actionReference.action.bindings[bindingIndex].overridePath);
			
			Dictionary<string, object> serializedData = new Dictionary<string, object>();

			settings.GetSerializeData(serializedData, new Setting[] { setting });

			byte[] savedData = serializer.Serialize(serializedData);

			setting.ResetToDefault(player);
			
			Assert.AreNotEqual(actionReference.action.bindings[bindingIndex].overridePath, j);

			serializedData.Clear();

			serializer.Deserialize(savedData, serializedData);

			Assert.IsTrue(serializedData.TryGetValue("setting", out object value));

			setting.SetSerializedValue(value, serializer);
			Assert.AreEqual(actionReference.action.bindings[bindingIndex].overridePath, j);
		}
	}
}
#endif