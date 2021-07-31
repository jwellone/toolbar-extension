using UnityEngine;
using UnityEditor;
using UniExtensions.Editor.Toolbar;

namespace Sample.Editor
{
	[InitializeOnLoadAttribute]
	public static class SampleToolbarExtension
	{
		static SampleToolbarExtension()
		{
			ToolbarExtension.AddLeftArea(new ToolbarButton("button1", "Button", () =>
			{
				var btns = ToolbarExtension.Find<ToolbarButton>();
				Debug.Log("click button1");
			}));

			ToolbarExtension.AddLeftArea(new ToolbarLabel("label", "Label"));
			ToolbarExtension.AddLeftArea(new ToolbarPopup("popup", 0,
				new string[] { "A", "B", "C", "D" },
				(index) =>
				{
					Debug.Log($"index->{index}");
				}));


			ToolbarExtension.AddRightArea(new ToolbarSlider("slider"));

			ToolbarExtension.AddRightArea(new ToolbarToggle("toggle", "toggle"));

			ToolbarExtension.AddRightArea(new ToolbarGradientField("gradient", new Gradient()));
		}
	}
}
