using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace jwellone.Toolbar.Editor
{
	using IGUI = IToolbarUI;

	public static class ToolbarExtension
	{
		private const BindingFlags BINDING_ATTR = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		static readonly Type TOOLBAR_TYPE = typeof(EditorGUI).Assembly.GetType("UnityEditor.Toolbar");
		static readonly FieldInfo TOOLBAR_GET = TOOLBAR_TYPE.GetField("get");

#if UNITY_2019
		private static readonly Type GUI_VIEW_TYPE = typeof(EditorGUI).Assembly.GetType("UnityEditor.GUIView");
		private static readonly PropertyInfo GUI_VIEW_IMGUI_CONTAINER = GUI_VIEW_TYPE.GetProperty("imguiContainer", BINDING_ATTR);
		private static readonly FieldInfo IMGUI_CONTAINER_ON_GUI_HANDLER = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BINDING_ATTR);
#else
		private static Type GUI_VIEW_TYPE = typeof(EditorGUI).Assembly.GetType("UnityEditor.GUIView");
		private static PropertyInfo WINDOW_BACKEND = GUI_VIEW_TYPE.GetProperty("windowBackend", BINDING_ATTR);
		private static Type IWINDOW_BACKEND = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");
		private static PropertyInfo VISUAL_TREE = IWINDOW_BACKEND.GetProperty("visualTree", BINDING_ATTR);
		private static FieldInfo IMGUI_CONTAINER_ON_GUI_HANDLER = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BINDING_ATTR);
#endif

		const float AREA_HEIGHT = 22;
		const float LEFT_AREA_POS_X = 410;
		const float LEFT_AREA_OFFSET_POS_X = -72;
		const float RIGHT_AREA_OFFSET_POS_X = 28;
		const float RIGHT_AREA_OFFSET_WIDTH = -376;

		static List<IGUI> s_leftAreaGuis = new List<IGUI>();
		static List<IGUI> s_rightAreaGuis = new List<IGUI>();

		[InitializeOnLoadMethod]
		static void InitializeOnLoad()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			try
			{
				var toolbar = TOOLBAR_GET.GetValue(null);
				if (toolbar == null)
				{
					return;
				}

#if UNITY_2019
				var imguiContainer = GUI_VIEW_IMGUI_CONTAINER.GetValue(toolbar, null) as IMGUIContainer;
				var handler = IMGUI_CONTAINER_ON_GUI_HANDLER.GetValue(imguiContainer) as Action;
#else
				var visualElement = VISUAL_TREE.GetValue(WINDOW_BACKEND.GetValue(toolbar), null) as VisualElement;
				var imguiContainer = visualElement[0] as IMGUIContainer;
				var handler = IMGUI_CONTAINER_ON_GUI_HANDLER.GetValue(imguiContainer) as Action;
#endif

				handler -= OnGUI;
				handler += OnGUI;

				IMGUI_CONTAINER_ON_GUI_HANDLER.SetValue(imguiContainer, handler);
			}
			catch (Exception ex)
			{
				Debug.LogError($"[ToolBarExtension]{ex.ToString()}");
			}

			EditorApplication.update -= OnUpdate;
		}

		static void OnGUI()
		{
			var editorWidth = EditorGUIUtility.currentViewWidth;
			var centerX = editorWidth / 2f;

			var leftAreaWidth = centerX + LEFT_AREA_OFFSET_POS_X - LEFT_AREA_POS_X;
			GUILayout.BeginArea(new Rect(LEFT_AREA_POS_X, 4, leftAreaWidth, AREA_HEIGHT));

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			foreach (var ui in s_leftAreaGuis)
			{
				ui.OnGUI();
			}

			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			var rightAreaPosX = centerX + RIGHT_AREA_OFFSET_POS_X;
			var rightAreaWidth = editorWidth + RIGHT_AREA_OFFSET_WIDTH - rightAreaPosX;
			GUILayout.BeginArea(new Rect(rightAreaPosX, 4, rightAreaWidth, AREA_HEIGHT));

			GUILayout.BeginHorizontal();

			foreach (var ui in s_rightAreaGuis)
			{
				ui.OnGUI();
			}

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		public static void AddLeftArea(in IGUI ui)
		{
			s_leftAreaGuis.Add(ui);
		}

		public static void AddRightArea(in IGUI ui)
		{
			s_rightAreaGuis.Add(ui);
		}

		public static bool Remove(in IGUI ui)
		{
			var result = false;
			if (s_leftAreaGuis.Contains(ui))
			{
				result |= s_leftAreaGuis.Remove(ui);
			}

			if (s_rightAreaGuis.Contains(ui))
			{
				result |= s_rightAreaGuis.Remove(ui);
			}

			return result;
		}

		public static IGUI Find(string name)
		{
			var target = s_leftAreaGuis.Find(ui => ui.Name == name);
			if (target != null)
			{
				return target;
			}

			target = s_rightAreaGuis.Find(ui => ui.Name == name);
			if (target != null)
			{
				return target;
			}

			return null;
		}

		public static T Find<T>(string name) where T : class, IGUI
		{
			var targetType = typeof(T);
			var target = s_leftAreaGuis.Find(ui => ui.Name == name && ui.GetType() == targetType);
			if (target != null)
			{
				return target as T;
			}

			target = s_rightAreaGuis.Find(ui => ui.Name == name && ui.GetType() == targetType);
			if (target != null)
			{
				return target as T;
			}

			return null;
		}

		public static T[] Find<T>() where T : class, IGUI
		{
			var targetType = typeof(T);
			var left = s_leftAreaGuis.FindAll(ui => ui.GetType() == targetType);
			var right = s_rightAreaGuis.FindAll(ui => ui.GetType() == targetType);

			var array = new T[left.Count + right.Count];
			var index = 0;
			foreach (var t in left)
			{
				array[index++] = t as T;
			}

			foreach (var t in right)
			{
				array[index++] = t as T;
			}

			return array;
		}
	}
}
