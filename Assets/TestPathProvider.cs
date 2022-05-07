using System;
using System.Collections.Generic;
using Hertzole.SettingsManager;
using UnityEngine;

[Serializable]
public class TestPathProvider : ISettingPathProvider
{
	[SerializeField]
	private bool isDesktop = true;
	[SerializeField] 
	private string lolers = default;

	public string GetSettingsPath()
	{
		return Environment.GetFolderPath(isDesktop ? Environment.SpecialFolder.Desktop : Environment.SpecialFolder.MyPictures) + "/" + lolers;
	}
}

[Serializable]
public class TestSerializer : ISettingSerializer
{
	public byte[] Serialize(Dictionary<string, object> data)
	{
		throw new NotImplementedException();
	}

	public void Deserialize(byte[] data, Dictionary<string, object> dataToFill)
	{
		throw new NotImplementedException();
	}

	public T DeserializeType<T>(object data)
	{
		throw new NotImplementedException();
	}
}