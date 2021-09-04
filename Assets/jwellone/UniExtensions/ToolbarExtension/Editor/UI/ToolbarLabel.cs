using UnityEngine;

namespace jwellone.Toolbar.Editor
{
	public class ToolbarLabel : ToolbarUI
	{
		public string Text { get; set; }

		public ToolbarLabel(string name, string text = "text")
		{
			Name = name;
			Text = text;
		}

		public override void OnGUI()
		{
			GUILayout.Label(Text, GUILayout.Height(22));
		}
	}
}
