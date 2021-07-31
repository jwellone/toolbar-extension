using System;
using UnityEngine;

namespace UniExtensions.Editor.Toolbar
{
	public class ToolbarButton : ToolbarUI
	{
		public string Text { get; set; }
		public Action Callback { get; set; }

		public ToolbarButton(string name, string text = "button", Action callback = null)
		{
			Name = name;
			Text = text;
			Callback = callback;
		}

		public override void OnGUI()
		{
			if (GUILayout.Button(Text, GUILayout.Height(22)))
			{
				Callback?.Invoke();
			}
		}
	}
}
