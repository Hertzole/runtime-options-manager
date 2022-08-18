using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Hertzole.OptionsManager.Tests
{
	public class FileWriterTests : BaseTest
	{
		[Test]
		public void FileExists_FileWriter()
		{
			TestFileExists<FileWriter>();
		}

		[Test]
		public void WriteReadFile_FileWriter()
		{
			TestWriteReadFile<FileWriter>();
		}

		[Test]
		public void FileExists_InMemory()
		{
			TestFileExists<InMemoryWriter>();
		}

		[Test]
		public void WriteReadFile_InMemory()
		{
			TestWriteReadFile<InMemoryWriter>();
		}

		[Test]
		public void SetFileWriterUpdates()
		{
			FileWriter fileWriter = new FileWriter();
			settings.FileWriter = fileWriter;
			Assert.AreEqual(fileWriter, settings.FileWriter);
			Assert.AreEqual(fileWriter, settings.behavior.Writer);

			InMemoryWriter inMemoryWriter = new InMemoryWriter();
			settings.FileWriter = inMemoryWriter;
			Assert.AreEqual(inMemoryWriter, settings.FileWriter);
			Assert.AreEqual(inMemoryWriter, settings.behavior.Writer);
		}

		private void TestFileExists<T>() where T : class, ISettingWriter, new()
		{
			settings.FileWriter = new T();
			IntSetting setting = AddSetting<IntSetting>();
			setting.Identifier = "setting";
			setting.Value = 42;

			Assert.IsFalse(settings.FileWriter.FileExists(settings.ComputedSavePath));
			settings.SaveSettings();
			Assert.IsTrue(settings.FileWriter.FileExists(settings.ComputedSavePath));
		}

		private void TestWriteReadFile<T>() where T : class, ISettingWriter, new()
		{
			settings.FileWriter = new T();
			IntSetting setting = AddSetting<IntSetting>();
			setting.Identifier = "setting";
			setting.Value = 42;

			settings.SaveSettings();
			setting.Value = 0;
			Assert.AreEqual(0, setting.Value);
			settings.LoadSettings();
			Assert.AreEqual(42, setting.Value);
		}

		public class InMemoryWriter : ISettingWriter
		{
			private readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

			public bool FileExists(string path)
			{
				return files.ContainsKey(path);
			}

			public void WriteFile(string path, byte[] content)
			{
				if (!files.ContainsKey(path))
				{
					files.Add(path, null);
				}

				Debug.Log($"Write file to {path}");

				files[path] = content;
			}

			public byte[] ReadFile(string path)
			{
				return files[path];
			}
		}
	}
}