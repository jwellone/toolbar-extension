using System;
using UnityEngine;
using UnityEditor;

namespace jwellone.Toolbar.Editor
{
	public class ToolbarToggle : ToolbarUI
	{
		private bool m_enabled = false;
		private string m_text = string.Empty;

		protected float _width = -1;

		public bool Enabled
		{
			get => m_enabled;
			set
			{
				if (value != m_enabled)
				{
					m_enabled = value;
					Callback?.Invoke(m_enabled);
				}
			}
		}

		public string Text
		{
			get => m_text;
			set
			{
				m_text = value;
				_width = -1;
			}
		}

		public Action<bool> Callback { get; set; }

		public ToolbarToggle(string name, string text = "text", bool enabled = false, Action<bool> callback = null)
		{
			Name = name;
			m_enabled = enabled;
			m_text = text;
			Callback = callback;
		}

		public override void OnGUI()
		{
			if (_width < 0)
			{
				_width = GUI.skin.label.CalcSize(new GUIContent(Text)).x;
			}

			EditorGUILayout.LabelField(Text, GUILayout.Width(_width), GUILayout.Height(22));
			Enabled = EditorGUILayout.Toggle(string.Empty, Enabled, GUILayout.Width(16), GUILayout.Height(22));
		}
	}

	public class ToolbarToggleLeft : ToolbarToggle
	{
		public ToolbarToggleLeft(string name, string text = "text", bool enabled = false, Action<bool> callback = null)
			: base(name, text, enabled, callback)
		{
		}

		public override void OnGUI()
		{
			if (_width < 0)
			{
				_width = GUI.skin.label.CalcSize(new GUIContent(Text)).x + 16;
			}

			Enabled = EditorGUILayout.ToggleLeft(Text, Enabled, GUILayout.Width(_width), GUILayout.Height(22));
		}
	}
}
