using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

#nullable enable

namespace jwelloneEditor.Toolbar.Sample
{
	public class SampleToolbarBuild : ToolbarUI
	{
		public override void OnGUI()
		{
			if (GUILayout.Button("Build"))
			{
				var defaultBuildPlayerOptions = new BuildPlayerOptions();
				var options = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(defaultBuildPlayerOptions);
				var report = BuildPipeline.BuildPlayer(options);
				if (report.summary.result == BuildResult.Failed)
				{
					Debug.LogError($"Build Error.");
				}
				else if (report.summary.result == BuildResult.Succeeded)
				{
					Debug.Log($"Build Succeeded.");
					EditorUtility.RevealInFinder(report.summary.outputPath);
				}
			}
		}
	}
}