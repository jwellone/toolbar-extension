namespace UniExtensions.Editor.Toolbar
{
	public abstract class ToolbarUI : IToolbarUI
	{
		public string Name { get; set; }

		public abstract void OnGUI();
	}
}
