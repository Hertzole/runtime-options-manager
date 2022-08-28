using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class FullscreenModeTests : BaseTest
	{
		private static readonly FullScreenMode[] fullScreenModes =
		{
			FullScreenMode.Windowed,
			FullScreenMode.MaximizedWindow,
			FullScreenMode.ExclusiveFullScreen,
			FullScreenMode.FullScreenWindow
		};

		[Test]
		public void GetDropdownValues([ValueSource(nameof(boolValues))] bool exclusiveFullscreen,
			[ValueSource(nameof(boolValues))] bool borderlessFullscreen,
			[ValueSource(nameof(boolValues))] bool maximizedWindow,
			[ValueSource(nameof(boolValues))] bool windowed)
		{
			int count = 0;
			if (exclusiveFullscreen)
			{
				count++;
			}

			if (borderlessFullscreen)
			{
				count++;
			}

			if (maximizedWindow)
			{
				count++;
			}

			if (windowed)
			{
				count++;
			}

			FullscreenModeSetting setting = AddSetting<FullscreenModeSetting>();
			setting.ExclusiveFullscreen.isEnabled = exclusiveFullscreen;
			setting.BorderlessFullscreen.isEnabled = borderlessFullscreen;
			setting.MaximizedWindow.isEnabled = maximizedWindow;
			setting.Windowed.isEnabled = windowed;

			IReadOnlyList<(string text, Sprite icon)> values = setting.GetDropdownValues();
			Assert.AreEqual(count, values.Count);

			bool containsExclusiveFullscreen = false;
			bool containsBorderlessFullscreen = false;
			bool containsMaximizedWindow = false;
			bool containsWindowed = false;

			for (int i = 0; i < values.Count; i++)
			{
				if (values[i].text == setting.ExclusiveFullscreen.name)
				{
					containsExclusiveFullscreen = true;
				}
				else if (values[i].text == setting.BorderlessFullscreen.name)
				{
					containsBorderlessFullscreen = true;
				}
				else if (values[i].text == setting.MaximizedWindow.name)
				{
					containsMaximizedWindow = true;
				}
				else if (values[i].text == setting.Windowed.name)
				{
					containsWindowed = true;
				}
			}

			if (exclusiveFullscreen)
			{
				Assert.IsTrue(containsExclusiveFullscreen);
			}
			else
			{
				Assert.IsFalse(containsExclusiveFullscreen);
			}

			if (borderlessFullscreen)
			{
				Assert.IsTrue(containsBorderlessFullscreen);
			}
			else
			{
				Assert.IsFalse(containsBorderlessFullscreen);
			}

			if (maximizedWindow)
			{
				Assert.IsTrue(containsMaximizedWindow);
			}
			else
			{
				Assert.IsFalse(containsMaximizedWindow);
			}

			if (windowed)
			{
				Assert.IsTrue(containsWindowed);
			}
			else
			{
				Assert.IsFalse(containsWindowed);
			}
		}
		
		[Test]
		public void SetDropdownValue([ValueSource(nameof(boolValues))] bool exclusiveFullscreen,
			[ValueSource(nameof(boolValues))] bool borderlessFullscreen,
			[ValueSource(nameof(boolValues))] bool maximizedWindow,
			[ValueSource(nameof(boolValues))] bool windowed)
		{
			int count = 0;
			if (exclusiveFullscreen)
			{
				count++;
			}

			if (borderlessFullscreen)
			{
				count++;
			}

			if (maximizedWindow)
			{
				count++;
			}

			if (windowed)
			{
				count++;
			}

			FullscreenModeSetting setting = AddSetting<FullscreenModeSetting>();
			setting.ExclusiveFullscreen.isEnabled = exclusiveFullscreen;
			setting.BorderlessFullscreen.isEnabled = borderlessFullscreen;
			setting.MaximizedWindow.isEnabled = maximizedWindow;
			setting.Windowed.isEnabled = windowed;

			IReadOnlyList<(string text, Sprite icon)> values = setting.GetDropdownValues();
			Assert.AreEqual(count, values.Count);
			
			for (int i = 0; i < values.Count; i++)
			{
				setting.SetDropdownValue(i);
				Assert.AreEqual(setting.GetModeAtIndex(i).fullScreenMode, setting.Value);
				Assert.AreEqual(i, setting.GetDropdownValue());
			}
		}
	}
}