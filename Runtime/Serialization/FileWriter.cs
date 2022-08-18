using System;
using System.IO;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	[Serializable]
	public class FileWriter : ISettingWriter
	{
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public void WriteFile(string path, byte[] content)
		{
			string directory = Path.GetDirectoryName(path);

			if (string.IsNullOrEmpty(directory))
			{
				Debug.LogError($"There's no directory at {path}.");
				return;
			}

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			File.WriteAllBytes(path, content);
		}

		public byte[] ReadFile(string path)
		{
			return File.ReadAllBytes(path);
		}
	}
}