using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	public static class ToolbarExtension
	{
		const BindingFlags BINDING_ATTR = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		static readonly Type TOOLBAR_TYPE = typeof(EditorGUI).Assembly.GetType("UnityEditor.Toolbar");

		const float AREA_HEIGHT = 22;
		const float LEFT_AREA_POS_X = 410;
		const float LEFT_AREA_OFFSET_POS_X = -72;
		const float RIGHT_AREA_OFFSET_POS_X = 28;
		const float RIGHT_AREA_OFFSET_WIDTH = -376;

		static UnityEngine.Object? _toolbar;
		static MethodInfo? _repaintMethod;
		static List<ToolbarUI> uiList => ToolbarCacheData.uiList;

		static UnityEngine.Object? toolbar
		{
			get
			{
				if(_toolbar==null)
				{
					var toolbars = Resources.FindObjectsOfTypeAll(TOOLBAR_TYPE);
					if (toolbars.Length > 0)
					{
						_toolbar = toolbars[0];
					}
				}

				return _toolbar;
			}
		}

		static MethodInfo? repaintMethod
		{
			get
			{
				if(_repaintMethod==null)
				{
					_repaintMethod = toolbar?.GetType().GetMethod("RepaintToolbar", BindingFlags.Static | BindingFlags.NonPublic);
				}
				return _repaintMethod;
			}
		}

		[InitializeOnLoadMethod]
		static void OnInitializeOnLoadMethod()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			try
			{
#if UNITY_2021_1_OR_NEWER
				if (toolbar == null)
				{
					return;
				}

				var fieldInfo = toolbar.GetType().GetField("m_Root", BINDING_ATTR);
				var rootElement = fieldInfo?.GetValue(toolbar) as VisualElement;
				var areaNames = new[] { "ToolbarZoneLeftAlign", "ToolbarZoneRightAlign" };
				var actions = new Action[] { OnGUILeft, OnGUIRight };
				for (var i = 0; i < actions.Length; ++i)
				{
					var area = rootElement.Q(areaNames[i]);
					var element = new VisualElement()
					{
						style =
						{
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
				var toolbar = TOOLBAR_TYPE.GetField("get").GetValue(null);
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
				var imguiContainer = visualElement?[0] as IMGUIContainer;
				var handler = onGUIHandle.GetValue(imguiContainer) as Action;
#endif

				handler -= OnGUI;
				handler += OnGUI;

				onGUIHandle.SetValue(imguiContainer, handler);
#endif
			}
			catch (Exception ex)
			{
				Debug.LogError($"[ToolBarExtension]{ex}");
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
			foreach (var ui in uiList)
			{
				if (!ui.valid || ui.area != ToolbarUI.Area.Left)
				{
					continue;
				}

				ui.OnGUI();
			}

			GUILayout.EndHorizontal();
		}

		static void OnGUIRight()
		{
			GUILayout.BeginHorizontal();
			foreach (var ui in uiList)
			{
				if (!ui.valid || ui.area != ToolbarUI.Area.Right)
				{
					continue;
				}

				ui.OnGUI();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public static T[] Find<T>() where T : ToolbarUI
		{
			return ToolbarCacheData.Find<T>();
		}

		public static void Repaint()
		{
			repaintMethod?.Invoke(toolbar, null);
		}
	}
}