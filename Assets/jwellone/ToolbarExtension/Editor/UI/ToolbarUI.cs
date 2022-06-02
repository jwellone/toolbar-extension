using System;
using UnityEngine;
using UnityEditor;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	[Serializable]
	public abstract class ToolbarUI
	{
		public enum Area
		{
			Left,
			Right
		}

		[SerializeField] string _typeName;
		[SerializeField] bool _valid = true;
		[SerializeField] Area _area;

		public string typeName => _typeName;
		public bool valid => _valid;
		public virtual string name => GetType().Name;
		public Area area => _area;

		public ToolbarUI()
		{
			_typeName = GetType().FullName;
		}

		public ToolbarUI(Area area)
		{
			_area = area;
			_typeName = GetType().FullName;
		}

		public abstract void OnGUI();

		public virtual void OnProjectSettingsActivate()
		{
		}

		public virtual Rect OnProjectSettingsGUI(Rect rect)
		{
			rect.width = 16;
			_valid = EditorGUI.Toggle(rect, _valid);

			rect.x += rect.width;
			rect.width = 128;
			EditorGUI.LabelField(rect, name);

			var tempY = rect.y;
			rect.x += rect.width;
			rect.y += 2;
			rect.width = 64;
			_area = (Area)EditorGUI.EnumPopup(rect, _area);

			rect.y = tempY;

			return rect;
		}

		public virtual void OnProjectSettingsDeactivate()
		{
		}
	}
}