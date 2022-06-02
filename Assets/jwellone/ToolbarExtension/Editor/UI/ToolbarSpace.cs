using System;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace jwelloneEditor.Toolbar
{
    [Serializable]
    public class ToolbarSpace : ToolbarUI
    {
        [SerializeField] int _space = 10;
        public override void OnGUI()
        {
            EditorGUILayout.Space(_space);
        }

        public override Rect OnProjectSettingsGUI(Rect rect)
        {
            rect = base.OnProjectSettingsGUI(rect);

            rect.x += rect.width + 16;
            rect.y += 3;
            rect.width = 120;
            rect.height = 18;
            _space = EditorGUI.IntField(rect, _space);
            
            return rect;
        }
    }
}