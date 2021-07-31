using System;
using UnityEditor;
using UnityEngine;

namespace UniExtensions.Editor.Toolbar
{
	public class ToolbarSpace : ToolbarUI
	{
		public bool Expand { get; set; } = true;

		public float Width { get; set; } = 6f;

		public ToolbarSpace(string name = "", bool expand = true, float width = 6f)
		{
			Name = name;
			Expand = expand;
			Width = width;
		}

		public override void OnGUI()
		{
			EditorGUILayout.Space(Width, Expand);
		}
	}
}
