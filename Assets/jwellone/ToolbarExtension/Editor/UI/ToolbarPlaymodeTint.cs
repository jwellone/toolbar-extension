using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	public class ToolbarPlaymodeTint : ToolbarUI
	{
		const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		public override string name => "Playmode tint";

		Color _color;
		object? _objPlaymodetint;
		FieldInfo? _prefColorField;
		MethodInfo? _setColorMethod;

		public Color playmodeTint
		{
			get
			{
				return _objPlaymodetint == null || _prefColorField == null ? Color.white : (Color)_prefColorField.GetValue(_objPlaymodetint);
			}
			set
			{
				if (_objPlaymodetint == null || _prefColorField == null || _setColorMethod == null)
				{
					return;
				}

				_prefColorField.SetValue(_objPlaymodetint, value);
				var data = (string)_setColorMethod.Invoke(_objPlaymodetint, null);
				EditorPrefs.SetString(name, data);
			}
		}

		public ToolbarPlaymodeTint()
			: base()
		{
			var types = typeof(Editor).Assembly.GetTypes();
			var tPrefSettings = types.Where(_ => _.Name == "PrefSettings").FirstOrDefault();
			var tPrefColor = types.Where(_ => _.Name == "PrefColor").FirstOrDefault();
			if (tPrefSettings == null || tPrefColor == null)
			{
				return;
			}

			var prefsField = tPrefSettings.GetField("m_Prefs", BINDING_FLAGS);
			_prefColorField = tPrefColor.GetField("m_Color", BINDING_FLAGS);
			_setColorMethod = tPrefColor.GetMethod("ToUniqueString", BINDING_FLAGS);
			if (prefsField == null || _prefColorField == null || _setColorMethod == null)
			{
				return;
			}

			var prefs = (SortedList<string, object>)prefsField.GetValue(null);
			_objPlaymodetint = prefs[name];

			_color = playmodeTint;
		}

		public override void OnGUI()
		{
			var color = EditorGUILayout.ColorField(_color, GUILayout.Width(64));
			if (color != _color)
			{
				_color = color;
				playmodeTint = _color;
			}
		}

		public override Rect OnProjectSettingsGUI(Rect rect)
		{
			rect = base.OnProjectSettingsGUI(rect);
			rect.x += rect.width + 16;
			var color = EditorGUI.ColorField(rect, _color);
			if (color != _color)
			{
				_color = color;
				playmodeTint = _color;
			}

			return rect;
		}
	}
}