using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#nullable enable

namespace jwelloneEditor.Toolbar
{
	[Serializable]
	public class ToolbarPlay : ToolbarUI
	{
		[SerializeField] string _guid = string.Empty;

		public override string name => "Play Button";

		int _selectIndex;
		string[]? _guids;
		string[]? _paths;
		GUIContent _guiContent = new GUIContent { tooltip = "Play the specified scene." };

		public override void OnGUI()
		{
			_guiContent.text = Application.isPlaying ? "■" : "▶︎";
			if (GUILayout.Button(_guiContent, GUILayout.Height(22)))
			{
				var isPlaying = !Application.isPlaying;
				if (isPlaying)
				{
					var path = AssetDatabase.GUIDToAssetPath(_guid);
					var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
					if (sceneAsset != null)
					{
						EditorSceneManager.playModeStartScene = sceneAsset;
					}
					else
					{
						Debug.LogError($"指定されたSceneは存在しません. guid({_guid})");
					}
				}

				EditorApplication.isPlaying = isPlaying;
			}
		}

		public override void OnProjectSettingsActivate()
		{
			Setup();
		}

		public override Rect OnProjectSettingsGUI(Rect rect)
		{
			rect = base.OnProjectSettingsGUI(rect);

			Setup();

			var tmpY = rect.y;
			rect.x += rect.width + 16;
			rect.y += 2;
			rect.width = 256;
			_selectIndex = EditorGUI.Popup(rect, _selectIndex, _paths);
			rect.y = tmpY;

			if (_paths?.Length >= 0 && _paths.Length > _selectIndex)
			{
				_guid = _guids![_selectIndex];
			}
			else
			{
				_guid = string.Empty;
			}

			return rect;
		}

		public override void OnProjectSettingsDeactivate()
		{
			_guids = null;
			_paths = null;
		}

		void Setup()
		{
			if (_guids != null)
			{
				return;
			}

			_guids = AssetDatabase.FindAssets("t:scene");
			_selectIndex = 0;
			_paths = new string[_guids.Length];
			for (var i = 0; i < _guids.Length; ++i)
			{
				var guid = _guids[i];
				if (guid == _guid)
				{
					_selectIndex = i;
				}

				_paths[i] = AssetDatabase.GUIDToAssetPath(guid).Replace('/', '>');
			}
		}
	}
}