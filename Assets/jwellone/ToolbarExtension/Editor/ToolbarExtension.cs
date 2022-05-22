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
#if UNITY_2021_1_OR_NEWER
				var toolbars = Resources.FindObjectsOfTypeAll(TOOLBAR_TYPE);
				var toolbar = toolbars.Length > 0 ? toolbars[0] : null;
				if (toolbar == null)
				{
					return;
				}

				var fieldInfo = toolbar.GetType().GetField("m_Root", BINDING_ATTR);
				var rootElement = fieldInfo.GetValue(toolbar) as VisualElement;
				var areaNames = new string[] { "ToolbarZoneLeftAlign", "ToolbarZoneRightAlign" };
				var actions = new Action[] { OnGUILeft, OnGUIRight };
				for (var i = 0; i < actions.Length; ++i)
				{
					var area = rootElement.Q(areaNames[i]);
					var element = new VisualElement()
					{
						style = {
							flexGrow = 1,
							flexDirection = FlexDirection.Row,
						}
					};
					var container = new IMGUIContainer()
					{
						style =
						{
							flexGrow = 1
						}
					};
					container.onGUIHandler += actions[i];
					element.Add(container);
					area.Add(element);
				}
#else

				var toolbar = TOOLBAR_GET.GetValue(null);
				if (toolbar == null)
				{
					return;
				}

#if UNITY_2019
				var guiViewType = typeof(EditorGUI).Assembly.GetType("UnityEditor.GUIView");
				var guiViewImguiContainer = guiViewType.GetProperty("imguiContainer", BINDING_ATTR);
				var onGUIHandle = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BINDING_ATTR);
				var imguiContainer = guiViewImguiContainer.GetValue(toolbar, null) as IMGUIContainer;
				var handler = onGUIHandle.GetValue(imguiContainer) as Action;
#else
				var guiViewType = typeof(EditorGUI).Assembly.GetType("UnityEditor.GUIView");
				var windowBackend = guiViewType.GetProperty("windowBackend", BINDING_ATTR);
				var iWindowBackend = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");
				var visualTree = iWindowBackend.GetProperty("visualTree", BINDING_ATTR);
				var onGUIHandle = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BINDING_ATTR);
				var visualElement = visualTree.GetValue(windowBackend.GetValue(toolbar), null) as VisualElement;
				var imguiContainer = visualElement[0] as IMGUIContainer;
				var handler = onGUIHandle.GetValue(imguiContainer) as Action;
#endif

				handler -= OnGUI;
				handler += OnGUI;

				onGUIHandle.SetValue(imguiContainer, handler);
#endif
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

			OnGUILeft();

			GUILayout.EndArea();

			var rightAreaPosX = centerX + RIGHT_AREA_OFFSET_POS_X;
			var rightAreaWidth = editorWidth + RIGHT_AREA_OFFSET_WIDTH - rightAreaPosX;
			GUILayout.BeginArea(new Rect(rightAreaPosX, 4, rightAreaWidth, AREA_HEIGHT));

			OnGUIRight();

			GUILayout.EndArea();
		}

		static void OnGUILeft()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			foreach (var ui in s_leftAreaGuis)
			{
				ui.OnGUI();
			}

			GUILayout.EndHorizontal();
		}

		static void OnGUIRight()
		{
			GUILayout.BeginHorizontal();
			foreach (var ui in s_rightAreaGuis)
			{
				ui.OnGUI();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
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
