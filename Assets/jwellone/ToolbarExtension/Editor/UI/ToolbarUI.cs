namespace jwellone.Toolbar.Editor
{
	public abstract class ToolbarUI : IToolbarUI
	{
		public string Name { get; set; }

		public abstract void OnGUI();
	}
}
