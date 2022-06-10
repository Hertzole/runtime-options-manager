using Unity.CodeEditor;
using UnityEditor;

namespace GitTools
{
	public static class CreateSolution
	{
		public static void Sync()
		{
			AssetDatabase.Refresh();
			CodeEditor.CurrentEditor.SyncAll();
		}
	}
}