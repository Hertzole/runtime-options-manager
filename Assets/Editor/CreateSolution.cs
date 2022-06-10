#if UNITY_EDITOR

using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor;

namespace GitTools
{
	public static class Solution
	{
		[MenuItem("Tools/Sync")]
		public static void Sync()
		{
			ProjectGeneration projectGeneration = new ProjectGeneration();
			AssetDatabase.Refresh();
			projectGeneration.GenerateAndWriteSolutionAndProjects();
		}
	}
}
#endif