using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class SettingsManagerTests : BaseTest
	{
		[Test]
		public void AddCategoryIsLast()
		{
			SettingsCategory newCategory = settings.AddCategory("new category");
			Assert.AreEqual(settings.Categories[settings.Categories.Count - 1], newCategory);
		}
		
		[Test]
		public void InsertCategoryAtCorrectIndex()
		{
			SettingsCategory newCategory = settings.InsertCategory(0, "new category");
			Assert.AreEqual(settings.Categories[0], newCategory);
		}
		
		[Test]
		public void AddSettingIsLast()
		{
			SettingsCategory category = settings.AddCategory("new category");

			for (int i = 0; i < 10; i++)
			{
				IntSetting fillerSetting = ScriptableObject.CreateInstance<IntSetting>();
				category.AddSetting(fillerSetting);
			}
			
			IntSetting setting = ScriptableObject.CreateInstance<IntSetting>();
			category.AddSetting(setting);
			Assert.AreEqual(category.Settings[category.Settings.Count - 1], setting);
		}

		[Test]
		public void InsertSettingAtCorrectIndex()
		{
			SettingsCategory category = settings.AddCategory("new category");

			for (int i = 0; i < 10; i++)
			{
				IntSetting fillerSetting = ScriptableObject.CreateInstance<IntSetting>();
				category.AddSetting(fillerSetting);
			}
			
			IntSetting setting = ScriptableObject.CreateInstance<IntSetting>();
			category.InsertSetting(0, setting);
			Assert.AreEqual(category.Settings[0], setting);
		}
		
		[Test]
		public void RemoveCategory_Valid()
		{
			SettingsCategory newCategory = settings.AddCategory("new category");
			bool wasRemoved = settings.RemoveCategory(newCategory);
			Assert.IsTrue(wasRemoved);
			
			for (int i = 0; i < settings.Categories.Count; i++)
			{
				Assert.AreNotEqual(newCategory, settings.Categories[i]);
			}
		}
		
		[Test]
		public void RemoveCategory_Invalid()
		{
			SettingsCategory newCategory = new SettingsCategory();
			bool wasRemoved = settings.RemoveCategory(newCategory);
			Assert.IsFalse(wasRemoved);
		}
		
		[Test]
		public void RemoveFirstCategory()
		{
			SettingsCategory newCategory = settings.InsertCategory(0, "new category");
			Assert.IsTrue(settings.RemoveCategory(0));

			for (int i = 0; i < settings.Categories.Count; i++)
			{
				Assert.AreNotEqual(newCategory, settings.Categories[i]);
			}
		}
		
		[Test]
		public void RemoveSetting_Valid()
		{
			SettingsCategory category = settings.AddCategory("new category");

			for (int i = 0; i < 10; i++)
			{
				IntSetting fillerSetting = ScriptableObject.CreateInstance<IntSetting>();
				category.AddSetting(fillerSetting);
			}
			
			IntSetting setting = ScriptableObject.CreateInstance<IntSetting>();
			category.AddSetting(setting);

			Assert.IsTrue(category.RemoveSetting(setting));

			for (int i = 0; i < category.Settings.Count; i++)
			{
				Assert.AreNotEqual(setting, category.Settings[i]);
			}
		}
		
		[Test]
		public void RemoveSetting_Invalid()
		{
			SettingsCategory category = settings.AddCategory("new category");

			for (int i = 0; i < 10; i++)
			{
				IntSetting fillerSetting = ScriptableObject.CreateInstance<IntSetting>();
				category.AddSetting(fillerSetting);
			}
			
			IntSetting setting = ScriptableObject.CreateInstance<IntSetting>();

			Assert.IsFalse(category.RemoveSetting(setting));
		}
		
		[Test]
		public void RemoveFirstSetting()
		{
			SettingsCategory category = settings.AddCategory("new category");

			for (int i = 0; i < 10; i++)
			{
				IntSetting fillerSetting = ScriptableObject.CreateInstance<IntSetting>();
				category.AddSetting(fillerSetting);
			}
			
			IntSetting setting = ScriptableObject.CreateInstance<IntSetting>();
			category.InsertSetting(0, setting);

			Assert.IsTrue(category.RemoveSetting(0));
			
			for (int i = 0; i < category.Settings.Count; i++)
			{
				Assert.AreNotEqual(setting, category.Settings[i]);
			}
		}
	}
}