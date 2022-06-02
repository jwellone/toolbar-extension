using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	class ToolbarSettings : SettingsProvider
	{
		ReorderableList? _reorderableList;
		Vector2 _scrollPos;
		double _time;

		[SettingsProvider]
		public static SettingsProvider Create()
		{
			return new ToolbarSettings("jwellone/")
			{
				label = "Toolbar",
				keywords = new[] { "jwellone", "Toolbar" }
			};
		}

		ToolbarSettings(string path, SettingsScope scopes = SettingsScope.Project)
			: base(path, scopes)
		{
			_time = EditorApplication.timeSinceStartup;
		}

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			base.OnActivate(searchContext, rootElement);
			foreach (var ui in ToolbarCacheData.uiList)
			{
				ui.OnProjectSettingsActivate();
			}


			_reorderableList = CreateReorderableList();
		}

		public override void OnGUI(string searchContext)
		{
			base.OnGUI(searchContext);

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Export", GUILayout.Width(48)))
			{
				var path = EditorUtility.SaveFilePanel("Export", Application.dataPath + "/../", "toolbar_list", "settings");
				if (ToolbarCacheData.Export(path))
				{
					EditorUtility.RevealInFinder(path);
				}
			}

			if (GUILayout.Button("Import", GUILayout.Width(48)))
			{
				var path = EditorUtility.OpenFilePanel("Import", Application.dataPath + "/../", "settings");
				ToolbarCacheData.Import(path);
			}

			if (GUILayout.Button("Apply", GUILayout.Width(48)))
			{
				ToolbarCacheData.Apply();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(18);
			GUILayout.Label("Show", GUILayout.Width(142));
			GUILayout.Label("Area", GUILayout.Width(76));
			GUILayout.Label("Exclusive");
			EditorGUILayout.EndHorizontal();

			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

			_reorderableList?.DoLayoutList();

			EditorGUILayout.EndScrollView();

			if (EditorApplication.timeSinceStartup - _time > 0.1333333)
			{
				_time = EditorApplication.timeSinceStartup;
				ToolbarExtension.Repaint();
			}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			foreach (var ui in ToolbarCacheData.uiList)
			{
				ui.OnProjectSettingsDeactivate();
			}

			_reorderableList = null;
		}

		ReorderableList CreateReorderableList()
		{
			return new ReorderableList(ToolbarCacheData.uiList, typeof(ToolbarUI), true, false, true, true)
			{
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					ToolbarCacheData.uiList[index].OnProjectSettingsGUI(rect);
				},
				onAddDropdownCallback = (buttonRect, list) =>
				{
					var uis = ToolbarCacheData.types.
						Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass && typeof(ToolbarUI).IsAssignableFrom(t)).
						Select(t => (ToolbarUI)Activator.CreateInstance(t)).ToList();

					var menu = new GenericMenu();
					foreach (var ui in uis)
					{
						menu.AddItem(
							new GUIContent(ui.name),
							false,
							(target) => { ToolbarCacheData.uiList.Add((ToolbarUI)target); },
							ui);
					}

					menu.ShowAsContext();
				}
			};
		}
	}
}