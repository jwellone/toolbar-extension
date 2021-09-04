using System;
using UnityEngine;
using UnityEditor;

namespace jwellone.Toolbar.Editor
{
	public class ToolbarSlider : ToolbarUI
	{
		private float m_nowValue = 0f;

		public float NowValue
		{
			get => m_nowValue;
			set
			{
				var target = Math.Max(MinValue, Math.Min(value, MaxValue));
				if (m_nowValue != target)
				{
					m_nowValue = target;
					Callback?.Invoke(m_nowValue);
				}
			}
		}

		public float MinValue { get; set; }
		public float MaxValue { get; set; }
		public Action<float> Callback { get; set; }

		public ToolbarSlider(string name, float now = 0f, float min = 0f, float max = 100f, Action<float> callback = null)
		{
			Name = name;
			MinValue = min;
			MaxValue = max;
			NowValue = now;
			Callback = callback;
		}

		public override void OnGUI()
		{
			NowValue = EditorGUILayout.Slider(NowValue, MinValue, MaxValue, GUILayout.Width(105));
		}
	}
}
