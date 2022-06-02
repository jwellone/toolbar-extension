using System;
using UnityEngine;
using UnityEditor;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	[Serializable]
	public class ToolbarTargetFrameRate : ToolbarUI
	{
		[SerializeField] int _min = -1;
		[SerializeField] int _max = 120;

		public override string name => "Target Frame Rate";

		readonly GUILayoutOption[] _labelOptions = new[] { GUILayout.Width(120), GUILayout.Height(7) };
		readonly GUILayoutOption[] _sliderOptions = new[] { GUILayout.Width(120), GUILayout.Height(15) };

		public ToolbarTargetFrameRate() : base(Area.Right)
		{
		}

		public override void OnGUI()
		{
			var labelStyle = new GUIStyle(EditorStyles.label) { fontSize = 7, alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Bold };
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(name, labelStyle, _labelOptions);
			Application.targetFrameRate = EditorGUILayout.IntSlider("", Application.targetFrameRate, _min, _max, _sliderOptions);
			EditorGUILayout.EndVertical();
		}

		public override Rect OnProjectSettingsGUI(Rect rect)
		{
			rect = base.OnProjectSettingsGUI(rect);

			var y = rect.y;

			rect.x += rect.width + 16;
			rect.width = 32;
			EditorGUI.LabelField(rect, "min");

			rect.x += rect.width;
			rect.y = y + 3;
			rect.width = 88;
			rect.height = 18;
			_min = EditorGUI.IntField(rect, "", _min);

			rect.x += rect.width + 16;
			rect.y = y;
			rect.width = 32;
			EditorGUI.LabelField(rect, "max");

			rect.x += rect.width;
			rect.y = y + 3;
			rect.width = 88;
			rect.height = 18;
			_max = EditorGUI.IntField(rect, "", _max);

			return rect;
		}
	}
}