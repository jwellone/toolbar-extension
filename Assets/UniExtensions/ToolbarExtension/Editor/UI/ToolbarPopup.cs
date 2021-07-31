using System;
using UnityEngine;
using UnityEditor;

namespace UniExtensions.Editor.Toolbar
{
	public class ToolbarPopup : ToolbarUI
	{
		private int m_index = 0;
		private string[] m_options;

		public int SelectIndex
		{
			get => m_index;
			set
			{
				if (m_index != value)
				{
					m_index = value;
					Callback?.Invoke(m_index);
				}
			}
		}

		public string[] Options
		{
			get => m_options;
			set
			{
				m_options = value;
				Width = -1;

				var index = m_index;
				if (m_options == null || m_options.Length <= 0)
				{
					index = 0;
				}
				else if (m_options.Length <= index)
				{
					index = m_options.Length - 1;
				}

				SelectIndex = index;
			}
		}

		private float Width { get; set; } = -1;
		public Action<int> Callback { get; set; }

		public ToolbarPopup(string name, int index, string[] options, Action<int> callback = null)
		{
			Name = name;
			SelectIndex = index;
			Options = options;
			Callback = callback;
		}

		public override void OnGUI()
		{
			if (Width < 0)
			{
				foreach (var option in Options)
				{
					var w = GUI.skin.label.CalcSize(new GUIContent(option)).x + 16;
					if (w > Width)
					{
						Width = w;
					}
				}
			}

			SelectIndex = EditorGUILayout.Popup(string.Empty, SelectIndex, Options, GUILayout.Width(Width));
		}
	}
}
