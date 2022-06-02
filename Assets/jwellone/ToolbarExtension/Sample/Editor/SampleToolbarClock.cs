using System;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace jwelloneEditor.Toolbar.Sample
{
	public class SampleToolbarClock : ToolbarUI
	{
		[SerializeField] string _format = "yyyy/MM/dd HH:mm";
		
		public override void OnGUI()
		{
			GUI.color = Color.yellow;
			EditorGUILayout.LabelField(DateTime.Now.ToString(_format), GUILayout.Width(108));
			GUI.color = Color.white;
		}

		public override Rect OnProjectSettingsGUI(Rect rect)
		{
			rect = base.OnProjectSettingsGUI(rect);
			rect.x += rect.width + 16;
			rect.y += 3;
			rect.height = 18;
			rect.width = 120;
			GUI.color = Color.yellow;
			_format = GUI.TextField(rect, _format);
			GUI.color = Color.white;
			return rect;
		}
	}
}