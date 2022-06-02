using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	[InitializeOnLoad]
	public static class ToolbarCacheData
	{
		const string USER_KEY_TOOLBAR_DATA = "USER_KEY_TOOLBAR_DATA";

		static Type[]? _types;
		public static List<ToolbarUI> uiList = new List<ToolbarUI>();

		public static Type[] types
		{
			get
			{
				if (_types == null)
				{
					_types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
					{
						if (a.FullName.StartsWith("Unity") || a.FullName.StartsWith("System"))
						{
							return Type.EmptyTypes;
						}
						
						return a.GetTypes();
					}).ToArray();
				}

				return _types;
			}
		}

		static ToolbarCacheData()
		{
			EditorApplication.delayCall -= OnInit;
			EditorApplication.delayCall += OnInit;
		}

		static void OnInit()
		{
			try
			{
				uiList.Clear();

				var tag = "_typeName:";
				var args = EditorUserSettings.GetConfigValue(USER_KEY_TOOLBAR_DATA).Split('\n');
				foreach (var arg in args)
				{
					foreach (var param in arg.Replace("\"", "").Replace("{", "").Replace("}", "").Split(','))
					{
						if (!param.Contains(tag))
						{
							continue;
						}

						try
						{
							var typeName = param.Replace(tag, "");
							var type = Type.GetType(typeName);
							if (type == null)
							{
								type = types.FirstOrDefault(t => t.FullName == typeName);
							}
							
							var ui = JsonUtility.FromJson(arg, type) as ToolbarUI;
							uiList.Add(ui!);
						}
						catch(Exception ex)
						{
							Debug.LogError($"[Toolbar]typeName({param.Replace(tag, "")}) {ex}");
						}
					}
				}
			}
			catch
			{
				uiList.Clear();
			}

			if (uiList.Count == 0)
			{
				uiList = new List<ToolbarUI>(new ToolbarUI[]
				{
					new ToolbarPlay(),
					new ToolbarTargetFrameRate(),
					new ToolbarTimeScale()
				});

				Apply();
			}
		}

		public static bool Export(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			try
			{
				var sb = new StringBuilder();
				foreach (var ui in uiList)
				{
					sb.AppendLine(JsonUtility.ToJson(ui));
				}

				File.WriteAllText(path, sb.ToString());
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool Import(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			try
			{
				var text = File.ReadAllText(path);
				EditorUserSettings.SetConfigValue(USER_KEY_TOOLBAR_DATA, text);
				OnInit();
			}
			catch
			{
				return false;
			}
			
			return true;
		}

		public static void Apply()
		{
			var sb = new StringBuilder();
			foreach (var ui in uiList)
			{
				sb.AppendLine(JsonUtility.ToJson(ui));
			}

			EditorUserSettings.SetConfigValue(USER_KEY_TOOLBAR_DATA, sb.ToString());
		}

		public static T[] Find<T>() where T : ToolbarUI
		{
			var targetType = typeof(T);
			var list = uiList.FindAll(ui => ui.GetType() == targetType);
			var array = new T[list.Count];
			for (var i = 0; i < list.Count; ++i)
			{
				array[i] = (T)list[i];
			}

			return array;
		}
	}
}