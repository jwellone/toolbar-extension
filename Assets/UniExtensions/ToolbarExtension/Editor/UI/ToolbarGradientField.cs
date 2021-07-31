using UnityEngine;
using UnityEditor;

namespace UniExtensions.Editor.Toolbar
{
	public class ToolbarGradientField : ToolbarUI
	{
		private Gradient m_gradient;

		public float Width
		{
			get;
			set;
		} = 32f;

		public Gradient Gradient
		{
			get => m_gradient;
			set
			{
				m_gradient = value;
			}
		}

		public ToolbarGradientField(string name, in Gradient gradient)
		{
			Name = name;
			m_gradient = gradient;
		}

		public override void OnGUI()
		{
			m_gradient = EditorGUILayout.GradientField(m_gradient, GUILayout.Width(Width));
		}
	}
}
