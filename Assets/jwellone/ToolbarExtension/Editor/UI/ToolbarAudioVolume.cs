using UnityEngine;
using UnityEditor;

#nullable enable

namespace jwelloneEditor.Toolbar
{
    public class ToolbarAudioVolume : ToolbarUI
    {
		const string CONFIG_NAME = "TOOLBAR_AUDIO_VOLUME";

		public override string name => "Audio Volume";

		readonly GUILayoutOption[] _labelOptions = new[] { GUILayout.Width(120), GUILayout.Height(7) };
		readonly GUILayoutOption[] _sliderOptions = new[] { GUILayout.Width(120), GUILayout.Height(15) };

		static float _volume = float.MaxValue;

		static float volume
        {
			get
            {
				if(_volume==float.MaxValue)
                {
					var config = EditorUserSettings.GetConfigValue(CONFIG_NAME);
					_volume = string.IsNullOrEmpty(config) ? 1f : float.Parse(config);
				}
				return _volume;
            }
			set
            {
				if(_volume == value)
                {
					return;
                }

				_volume = value;
				AudioListener.volume = value;
				EditorUserSettings.SetConfigValue(CONFIG_NAME, value.ToString());
			}
        }

		public override void OnGUI()
		{
			var labelStyle = new GUIStyle(EditorStyles.label) { fontSize = 7, alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Bold };
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(name, labelStyle, _labelOptions);
			volume = EditorGUILayout.Slider("", volume, 0f, 1f, _sliderOptions);
			EditorGUILayout.EndVertical();
		}

		[InitializeOnLoadMethod]
		static void OnInitializeOnLoadMethod()
        {
			AudioListener.volume = volume;
		}
	}
}
